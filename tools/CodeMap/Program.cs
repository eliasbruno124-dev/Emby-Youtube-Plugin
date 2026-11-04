using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// Syntax inventory, deliberately not a compiler-bound call graph. The plugin
// and Emby's private assemblies are not loaded or executed by this program.
if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: CodeMap <repository-root> <prepared-input.json> <output.json>");
    return 2;
}

try
{
    var root = Path.GetFullPath(args[0]);
    using var input = JsonDocument.Parse(File.ReadAllText(args[1]));
    if (input.RootElement.GetProperty("schemaVersion").GetInt32() != 1)
        throw new InvalidOperationException("Unsupported prepared-input schema.");
    var files = new List<object>();
    var paths = new HashSet<string>(StringComparer.Ordinal);
    foreach (var entry in input.RootElement.GetProperty("files").EnumerateArray())
    {
        var relative = entry.GetProperty("path").GetString() ?? throw new InvalidOperationException("Missing path.");
        if (!relative.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
            || Path.IsPathRooted(relative) || relative.Contains('\\')
            || relative.Split('/').Any(part => part is "" or "." or "..")
            || !paths.Add(relative))
            throw new InvalidOperationException("Unsafe or duplicate C# input path.");
        var path = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        var bytes = File.ReadAllBytes(path);
        var sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        if (sha256 != entry.GetProperty("sha256").GetString())
            throw new InvalidOperationException($"Prepared input changed: {relative}");
        var source = Encoding.UTF8.GetString(bytes);
        if (source.StartsWith('\uFEFF')) source = source[1..];
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest), relative);
        if (tree.GetRoot().DescendantTrivia().Any(trivia => trivia.IsKind(SyntaxKind.DisabledTextTrivia)))
            throw new InvalidOperationException($"Inactive conditional C# source needs explicit inventory support: {relative}");
        var errors = tree.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
        if (errors.Length != 0)
            throw new InvalidOperationException($"C# syntax error in {relative}: "
                + string.Join("; ", errors.Take(5).Select(d => d.Id + " at line "
                    + (d.Location.GetLineSpan().StartLinePosition.Line + 1))));
        var declarations = new List<object>();
        var payloads = new List<object>();
        foreach (var node in tree.GetRoot().DescendantNodesAndSelf())
        {
            var description = Describe(node);
            if (description is { } item)
            {
                var location = tree.GetLineSpan(node.Span);
                var owners = node.Ancestors().Reverse().Select(Describe)
                    .Where(value => value is not null)
                    .Select(value => value!.Value.Name).ToArray();
                declarations.Add(new
                {
                    kind = item.Kind,
                    name = item.Name,
                    owner = string.Join(".", owners),
                    line = location.StartLinePosition.Line + 1,
                    column = location.StartLinePosition.Character + 1,
                    endLine = location.EndLinePosition.Line + 1,
                    start = node.SpanStart,
                    length = node.Span.Length,
                    signature = Signature(node),
                    syntaxKind = node.Kind().ToString()
                });
            }
            if (node is VariableDeclaratorSyntax variable
                && variable.Identifier.ValueText is "PlayerPatchHelpers" or "IframeTeardownHelper" or "legacyAbortHelper"
                && variable.Initializer?.Value is ExpressionSyntax value)
            {
                var payload = ConstantString(value)
                    ?? throw new InvalidOperationException(variable.Identifier.ValueText + " is no longer a supported constant string concatenation.");
                payloads.Add(new
                {
                    name = variable.Identifier.ValueText,
                    line = tree.GetLineSpan(node.Span).StartLinePosition.Line + 1,
                    source = payload
                });
            }
        }
        files.Add(new { path = relative, sha256, declarations, javascriptPayloads = payloads });
    }
    var output = new
    {
        schemaVersion = 1,
        engine = "Roslyn syntax tree (not semantic binding)",
        engineVersion = typeof(CSharpSyntaxTree).Assembly.GetName().Version?.ToString(),
        languageVersion = "Latest supported by recorded Roslyn version",
        files
    };
    var destination = Path.GetFullPath(args[2]);
    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
    File.WriteAllText(destination, JsonSerializer.Serialize(output,
        new JsonSerializerOptions { WriteIndented = true }) + "\n", new UTF8Encoding(false));
    Console.WriteLine($"CODE_MAP_CSHARP_PARSED files={files.Count}");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine("CodeMap failed: " + exception.Message);
    return 1;
}

static (string Kind, string Name)? Describe(SyntaxNode node) => node switch
{
    BaseNamespaceDeclarationSyntax value => ("namespace", value.Name.ToString()),
    BaseTypeDeclarationSyntax value => ("type", value.Identifier.ValueText),
    DelegateDeclarationSyntax value => ("delegate", value.Identifier.ValueText),
    MethodDeclarationSyntax value => ("method", (value.ExplicitInterfaceSpecifier?.Name + ".").TrimStart('.') + value.Identifier.ValueText),
    ConstructorDeclarationSyntax value => ("constructor", value.Identifier.ValueText),
    DestructorDeclarationSyntax value => ("destructor", "~" + value.Identifier.ValueText),
    OperatorDeclarationSyntax value => ("operator", "operator " + value.OperatorToken.ValueText),
    ConversionOperatorDeclarationSyntax value => ("conversion", value.ImplicitOrExplicitKeyword.ValueText + " operator " + value.Type),
    PropertyDeclarationSyntax value => ("property", (value.ExplicitInterfaceSpecifier?.Name + ".").TrimStart('.') + value.Identifier.ValueText),
    IndexerDeclarationSyntax value => ("indexer", (value.ExplicitInterfaceSpecifier?.Name + ".").TrimStart('.') + "this[]"),
    EventDeclarationSyntax value => ("event", value.Identifier.ValueText),
    EnumMemberDeclarationSyntax value => ("enum-member", value.Identifier.ValueText),
    LocalFunctionStatementSyntax value => ("local-function", value.Identifier.ValueText),
    VariableDeclaratorSyntax value => (value.Parent?.Parent is EventFieldDeclarationSyntax ? "event"
        : value.Parent?.Parent is FieldDeclarationSyntax ? "field" : "local-variable", value.Identifier.ValueText),
    ParameterSyntax value => ("parameter", value.Identifier.ValueText),
    TypeParameterSyntax value => ("type-parameter", value.Identifier.ValueText),
    TupleElementSyntax value when !value.Identifier.IsKind(SyntaxKind.None) => ("tuple-element", value.Identifier.ValueText),
    AccessorDeclarationSyntax value => ("accessor", value.Keyword.ValueText),
    AnonymousFunctionExpressionSyntax => ("anonymous-function", "anonymous@" + node.SpanStart),
    ForEachStatementSyntax value => ("iteration-variable", value.Identifier.ValueText),
    CatchDeclarationSyntax value when !value.Identifier.IsKind(SyntaxKind.None) => ("catch-variable", value.Identifier.ValueText),
    SingleVariableDesignationSyntax value => ("pattern-variable", value.Identifier.ValueText),
    LabeledStatementSyntax value => ("label", value.Identifier.ValueText),
    UsingDirectiveSyntax value when value.Alias != null => ("using-alias", value.Alias.Name.Identifier.ValueText),
    ExternAliasDirectiveSyntax value => ("extern-alias", value.Identifier.ValueText),
    FromClauseSyntax value => ("query-variable", value.Identifier.ValueText),
    LetClauseSyntax value => ("query-variable", value.Identifier.ValueText),
    JoinClauseSyntax value => ("query-variable", value.Identifier.ValueText),
    JoinIntoClauseSyntax value => ("query-variable", value.Identifier.ValueText),
    QueryContinuationSyntax value => ("query-variable", value.Identifier.ValueText),
    _ => null
};

static string Signature(SyntaxNode node)
{
    // Stop before executable bodies/initializers. Token normalization removes
    // trivia but retains parameter types, overloads, modifiers and constraints.
    var end = node switch
    {
        BaseMethodDeclarationSyntax value => value.Body?.SpanStart ?? value.ExpressionBody?.SpanStart ?? value.Span.End,
        BaseTypeDeclarationSyntax value => value.OpenBraceToken.SpanStart > value.SpanStart ? value.OpenBraceToken.SpanStart : value.Span.End,
        BaseNamespaceDeclarationSyntax value => value.Name.Span.End,
        BasePropertyDeclarationSyntax value => value.AccessorList?.SpanStart ?? value.Span.End,
        LocalFunctionStatementSyntax value => value.Body?.SpanStart ?? value.ExpressionBody?.SpanStart ?? value.Span.End,
        VariableDeclaratorSyntax value => value.Initializer?.SpanStart ?? value.Span.End,
        AnonymousFunctionExpressionSyntax value => value.Body.SpanStart,
        _ => node.Span.End
    };
    var tokens = node.DescendantTokens().Where(token => token.SpanStart < end).Select(token => token.Text);
    var signature = string.Join(" ", tokens);
    if (node is VariableDeclaratorSyntax && node.Parent is VariableDeclarationSyntax declaration)
    {
        var modifiers = declaration.Parent is BaseFieldDeclarationSyntax field
            ? string.Join(" ", field.Modifiers.Select(token => token.Text)) + " " : string.Empty;
        signature = modifiers + declaration.Type + " " + signature;
    }
    return signature.Length <= 500 ? signature : signature[..500] + " …";
}

static string? ConstantString(ExpressionSyntax expression) => expression switch
{
    LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression) => literal.Token.ValueText,
    BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AddExpression)
        && ConstantString(binary.Left) is string left
        && ConstantString(binary.Right) is string right => left + right,
    ParenthesizedExpressionSyntax parenthesized => ConstantString(parenthesized.Expression),
    _ => null
};
