using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

// Minimal framework-only compatibility methods for the pinned source subset.
// No external package, executable, native import or module initializer is needed.
namespace PowerKit.Extensions
{
    internal static class ManagedCompatibility
    {
        internal static TResult Pipe<T, TResult>(this T value, Func<T, TResult> map) => map(value);
        internal static string? NullIfWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
        internal static string SubstringAfter(this string value, string separator)
        { var index = value.IndexOf(separator, StringComparison.Ordinal); return index < 0 ? "" : value[(index + separator.Length)..]; }
        internal static string SubstringUntil(this string value, string separator)
        { var index = value.IndexOf(separator, StringComparison.Ordinal); return index < 0 ? value : value[..index]; }
        internal static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class => source.Where(value => value != null)!;
        internal static IEnumerable<(int Index, T Item)> Index<T>(this IEnumerable<T> source) => source.Select((value, index) => (index, value));
    }

    internal static class ManagedNumeric
    {
        internal static long? ParseLongOrNull(string value, IFormatProvider provider) =>
            long.TryParse(value, NumberStyles.Integer, provider, out long parsed) ? parsed : null;
        internal static double? ParseDoubleOrNull(string value, IFormatProvider provider) =>
            double.TryParse(value, NumberStyles.Float, provider, out double parsed) && double.IsFinite(parsed) ? parsed : null;
    }
}

namespace JsonExtensions
{
    internal static class Json
    {
        internal static JsonElement Parse(string source)
        { using var document = JsonDocument.Parse(source, new JsonDocumentOptions { MaxDepth = 48 }); return document.RootElement.Clone(); }
        internal static JsonElement? TryParse(string source)
        { try { return Parse(source); } catch (JsonException) { return null; } }
    }
}

namespace JsonExtensions.Reading
{
    internal static class ManagedJsonReading
    {
        internal static JsonElement? GetPropertyOrNull(this JsonElement value, string name) =>
            value.ValueKind == JsonValueKind.Object && value.TryGetProperty(name, out var found) ? found : null;
        internal static string? GetStringOrNull(this JsonElement value) => value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        internal static int? GetInt32OrNull(this JsonElement value) => value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int parsed) ? parsed : null;
        internal static long? GetInt64OrNull(this JsonElement value) => value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out long parsed) ? parsed : null;
        internal static bool? GetBooleanOrNull(this JsonElement value) => value.ValueKind == JsonValueKind.True ? true : value.ValueKind == JsonValueKind.False ? false : null;
        internal static IEnumerable<JsonElement>? EnumerateArrayOrNull(this JsonElement value) =>
            value.ValueKind == JsonValueKind.Array ? value.EnumerateArray().ToArray() : null;
    }
}
