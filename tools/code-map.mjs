#!/usr/bin/env node
import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import { createRequire } from 'node:module';
import { fileURLToPath } from 'node:url';

const require = createRequire(import.meta.url);
const DEFAULT_ROOT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const CONFIG = 'tools/code-map.config.json';
const json = value => JSON.stringify(value, null, 2) + '\n';
const hash = value => crypto.createHash('sha256').update(value).digest('hex');
const sorted = values => [...values].sort((a, b) => a < b ? -1 : a > b ? 1 : 0);
const assert = (condition, message) => { if (!condition) throw new Error(message); };
const readJson = filename => JSON.parse(fs.readFileSync(filename, 'utf8'));
const sourceLink = (filename, line) => '../' + filename.split('/').map(encodeURIComponent).join('/') + (line ? '#L' + line : '');
const escapeMd = value => String(value).replaceAll('|', '\\|').replaceAll('\n', ' ').replaceAll('`', '\\`').replaceAll('<', '&lt;').replaceAll('>', '&gt;');

function safeRelative(value) {
    assert(typeof value === 'string' && value.length && !path.isAbsolute(value)
        && !value.includes('\\') && !value.split('/').some(part => ['', '.', '..'].includes(part)), 'Unsafe relative map path');
    return value;
}

export function loadConfig(root) {
    const config = readJson(path.join(root, CONFIG));
    assert(config.schemaVersion === 1, 'Unsupported configuration schema');
    for (const key of ['excludedDirectories', 'excludedFiles', 'generatedOutputs']) assert(Array.isArray(config[key]), 'Missing config ' + key);
    for (const value of [...config.excludedFiles, ...config.generatedOutputs, config.reviewFile, config.flowFile, config.guideFile, config.pluginProject]) safeRelative(value);
    for (const value of config.excludedDirectories) assert(typeof value === 'string' && value && !/[\\/]/.test(value), 'Invalid excluded directory');
    assert(config.generatedOutputs.join('|') === 'docs/CODE_MAP.md|docs/code-map.json|docs/code-map.manifest.json', 'Unsupported output layout');
    return config;
}

export function snapshot(root, config = loadConfig(root)) {
    const excludedFiles = new Set([...config.excludedFiles, ...config.generatedOutputs, config.reviewFile]);
    const excludedDirectories = new Set(config.excludedDirectories);
    const files = [];
    function visit(directory, prefix) {
        for (const name of sorted(fs.readdirSync(directory))) {
            const relative = prefix ? prefix + '/' + name : name;
            const absolute = path.join(directory, name);
            const stat = fs.lstatSync(absolute);
            if (excludedFiles.has(relative) || (stat.isDirectory() && excludedDirectories.has(name))) continue;
            assert(!stat.isSymbolicLink(), 'Symlink is outside map scope: ' + relative);
            if (stat.isDirectory()) visit(absolute, relative);
            else {
                assert(stat.isFile(), 'Non-regular map input: ' + relative);
                const bytes = fs.readFileSync(absolute);
                files.push({ path: relative, sha256: hash(bytes), bytes: bytes.length });
            }
        }
    }
    visit(root, '');
    files.sort((a, b) => a.path < b.path ? -1 : a.path > b.path ? 1 : 0);
    assert(files.some(file => file.path === CONFIG), 'Configuration excluded itself');
    return files;
}

export function prepare(root, destination) {
    const files = snapshot(root).filter(file => /\.cs$/i.test(file.path)).map(({ path, sha256 }) => ({ path, sha256 }));
    writeFile(destination, json({ schemaVersion: 1, files }));
    return files.length;
}

function writeFile(filename, content) {
    fs.mkdirSync(path.dirname(filename), { recursive: true });
    fs.writeFileSync(filename, content);
}

function globRegex(pattern) {
    pattern = pattern.replaceAll('\\', '/');
    let result = '^';
    for (let i = 0; i < pattern.length; i++) {
        if (pattern.slice(i, i + 3) === '**/') { result += '(?:.*/)?'; i += 2; }
        else if (pattern.slice(i, i + 2) === '**') { result += '.*'; i++; }
        else if (pattern[i] === '*') result += '[^/]*';
        else if (pattern[i] === '?') result += '[^/]';
        else result += pattern[i].replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }
    return new RegExp(result + '$');
}

function attributes(text) {
    const result = {};
    const regex = /([^\s=<>/]+)(?:\s*=\s*(?:"([^"]*)"|'([^']*)'|([^\s<>]+)))?/g;
    for (const match of text.matchAll(regex)) result[match[1].toLowerCase()] = match[2] ?? match[3] ?? match[4] ?? '';
    return result;
}

function projectFacts(root, config) {
    for (let directory = root; ; directory = path.dirname(directory)) {
        for (const name of ['Directory.Build.props', 'Directory.Build.targets'])
            assert(!fs.existsSync(path.join(directory, name)), 'Implicit ' + name + ' needs full MSBuild input evaluation');
        if (path.dirname(directory) === directory) break;
    }
    const source = fs.readFileSync(path.join(root, config.pluginProject), 'utf8').replace(/<!--[\s\S]*?-->/g, '');
    assert(/<Project\s+Sdk="Microsoft.NET.Sdk"/.test(source), 'Unsupported project SDK; review Compile input handling');
    assert(!/<(?:Import|Target|EnableDefaultItems|EnableDefaultCompileItems|DefaultItemExcludes|DefaultItemExcludesInProjectFolder|DirectoryBuildPropsPath|DirectoryBuildTargetsPath|BaseIntermediateOutputPath|BaseOutputPath)\b/i.test(source), 'Unsupported project input customization; update map evaluator');
    const removals = [];
    const resources = [];
    for (const match of source.matchAll(/<(Compile|EmbeddedResource|Content)\b([^>]*)>/g)) {
        const attrs = attributes(match[2]);
        assert(!('condition' in attrs) && !('exclude' in attrs) && !('update' in attrs), 'Conditional/updated project items need explicit map evaluator support');
        if (match[1] === 'Compile') {
            assert(typeof attrs.remove === 'string' && !('include' in attrs), 'Only SDK default Compile plus Remove is supported');
            assert(!/[$@%]\(/.test(attrs.remove), 'Compile expressions need full MSBuild input evaluation');
            removals.push(...attrs.remove.split(';').filter(Boolean).map(pattern => pattern.replaceAll('\\', '/')));
        } else {
            assert(typeof attrs.include === 'string', 'Unsupported resource item');
            resources.push({ kind: match[1], include: attrs.include.replaceAll('\\', '/'), logicalName: attrs.logicalname ?? null,
                externalExpression: attrs.include.includes('$(') });
        }
    }
    const conditionedGroups = [...source.matchAll(/<ItemGroup\b([^>]*)>([\s\S]*?)<\/ItemGroup>/g)]
        .some(match => 'condition' in attributes(match[1]) && /<(Compile|EmbeddedResource|Content)\b/.test(match[2]));
    assert(!conditionedGroups, 'Conditional project item groups need explicit map evaluator support');
    return { file: config.pluginProject, compileModel: 'SDK default C# Compile minus explicit Remove; configured inventory directories excluded',
        compileRemovals: removals, resources };
}

function validateExcludedCompileInputs(root, config, project) {
    const skipped = new Set(config.excludedDirectories);
    function visit(directory, prefix, excluded) {
        for (const name of fs.readdirSync(directory)) {
            const relative = prefix ? prefix + '/' + name : name;
            const filename = path.join(directory, name), stat = fs.lstatSync(filename);
            if (stat.isDirectory()) {
                // SDK default output paths are root bin/obj, not every nested
                // directory with those names. Hidden directories are default
                // SDK exclusions; explicit Compile Include is unsupported.
                if (name.startsWith('.') || (!prefix && ['bin', 'obj'].includes(name))) continue;
                visit(filename, relative, excluded || skipped.has(name));
            } else if (stat.isFile() && /\.cs$/i.test(name) && (excluded || config.excludedFiles.includes(relative))) {
                assert(project.compileRemovals.some(pattern => globRegex(pattern).test(relative)),
                    'Inventory excludes C# that may compile into the plugin: ' + relative);
            }
        }
    }
    visit(root, '', false);
}

function buildRole(filename, project) {
    if (/\.cs$/i.test(filename) && filename.split('/').slice(0, -1).some(part => part.startsWith('.')))
        return 'C# source excluded by SDK hidden-directory default';
    if (/\.cs$/i.test(filename)) return project.compileRemovals.some(pattern => globRegex(pattern).test(filename))
        ? 'C# source excluded from plugin Compile' : 'Plugin Compile source (SDK default)';
    const roles = project.resources.filter(item => !item.externalExpression && globRegex(item.include).test(filename)).map(item => item.kind);
    if (roles.length) return roles.join(' + ');
    if (/\.(csproj|sln)$/.test(filename)) return 'Build definition';
    if (filename.startsWith('tools/')) return 'Development tooling';
    if (filename.startsWith('docs/') || /\.(md|png)$/i.test(filename)) return 'Documentation/resource';
    return 'Repository support';
}

function bindingNames(node) {
    if (!node) return [];
    if (node.type === 'Identifier') return [node];
    if (node.type === 'RestElement') return bindingNames(node.argument);
    if (node.type === 'AssignmentPattern') return bindingNames(node.left);
    if (node.type === 'ArrayPattern') return node.elements.flatMap(bindingNames);
    if (node.type === 'ObjectPattern') return node.properties.flatMap(property => bindingNames(property.value ?? property.argument));
    return [];
}

function javascriptSymbols(source, acorn, { lineOffset = 0, columnOffset = 0, owner = '', sourceType = 'script', payloadLine = null } = {}) {
    let ast;
    const parseOptions = { ecmaVersion: 'latest', sourceType: sourceType === 'unambiguous' ? 'script' : sourceType, locations: true, allowHashBang: true };
    try { ast = acorn.parse(source, parseOptions); }
    catch (error) {
        if (sourceType !== 'unambiguous') throw error;
        ast = acorn.parse(source, { ...parseOptions, sourceType: 'module' });
    }
    const symbols = [];
    function add(node, kind, name, context) {
        symbols.push({ kind, name, owner: context.join('.'), line: payloadLine ?? node.loc.start.line + lineOffset,
            column: node.loc.start.column + 1 + (node.loc.start.line === 1 ? columnOffset : 0),
            endLine: payloadLine ?? node.loc.end.line + lineOffset, start: node.start, length: node.end - node.start,
            signature: source.slice(node.start, Math.min(node.end, node.start + 160)).split('\n')[0],
            language: payloadLine ? 'JavaScript in C# constant' : 'JavaScript',
            ...(payloadLine ? { payloadLine: node.loc.start.line, payloadColumn: node.loc.start.column + 1 } : {}) });
    }
    function assignedName(node, parent) {
        if (node.id?.name) return node.id.name;
        if (parent?.type === 'VariableDeclarator' && parent.id.type === 'Identifier') return parent.id.name;
        if (parent?.type === 'Property' || parent?.type === 'MethodDefinition') return String(parent.key.name ?? parent.key.value ?? 'computed');
        if (parent?.type === 'AssignmentExpression') return source.slice(parent.left.start, parent.left.end);
        if (parent?.type === 'CallExpression') return source.slice(parent.callee.start, parent.callee.end) + ' callback@' + node.start;
        return 'anonymous@' + node.start;
    }
    function walk(node, parent, context) {
        if (!node || typeof node.type !== 'string') return;
        let children = context;
        if (/^(FunctionDeclaration|FunctionExpression|ArrowFunctionExpression)$/.test(node.type)) {
            const name = assignedName(node, parent);
            if (parent?.type !== 'MethodDefinition') add(node, 'function', name, context);
            children = parent?.type === 'MethodDefinition' ? context : [...context, name];
            for (const parameter of node.params.flatMap(bindingNames)) add(parameter, 'parameter', parameter.name, children);
        } else if (/^Class(?:Declaration|Expression)$/.test(node.type)) {
            const name = node.id?.name ?? 'anonymous-class@' + node.start;
            add(node, 'class', name, context); children = [...context, name];
        } else if (node.type === 'MethodDefinition') {
            const name = String(node.key.name ?? node.key.value ?? 'computed@' + node.start);
            add(node, 'method', name, context); children = [...context, name];
        } else if (node.type === 'VariableDeclarator') {
            for (const identifier of bindingNames(node.id)) add(identifier, 'variable', identifier.name, context);
        } else if (node.type === 'CatchClause') {
            for (const identifier of bindingNames(node.param)) add(identifier, 'catch-variable', identifier.name, context);
        } else if (/^Import(?:Default|Namespace)?Specifier$/.test(node.type)) {
            add(node.local, 'import', node.local.name, context);
        } else if (node.type === 'PropertyDefinition') {
            add(node, 'class-field', String(node.key.name ?? node.key.value ?? 'computed@' + node.start), context);
        }
        for (const [key, value] of Object.entries(node)) {
            if (['loc', 'start', 'end', 'type'].includes(key)) continue;
            if (Array.isArray(value)) for (const child of value) walk(child, node, children);
            else if (value && typeof value.type === 'string') walk(value, node, children);
        }
    }
    walk(ast, null, owner ? [owner] : []);
    return symbols;
}

function locationAt(source, offset) {
    const prefix = source.slice(0, offset);
    const line = prefix.split('\n').length;
    return { line, column: offset - prefix.lastIndexOf('\n') };
}

function htmlInventory(source, acorn) {
    const symbols = [], resources = [];
    let cursor = 0;
    // A bounded HTML token scanner, not a DOM validator. It respects quoted
    // tag attributes, comments and raw script/style text before Acorn parsing.
    while (cursor < source.length) {
        const start = source.indexOf('<', cursor);
        if (start < 0) break;
        if (source.startsWith('<!--', start)) {
            const end = source.indexOf('-->', start + 4);
            assert(end >= 0, 'Unterminated HTML comment'); cursor = end + 3; continue;
        }
        let end = start + 1, quote = '';
        for (; end < source.length; end++) {
            const char = source[end];
            if (quote) { if (char === quote) quote = ''; }
            else if (char === '"' || char === "'") quote = char;
            else if (char === '>') break;
        }
        assert(end < source.length, 'Unterminated HTML tag');
        cursor = end + 1;
        const match = /^<([A-Za-z][\w:-]*)\b([\s\S]*?)>$/.exec(source.slice(start, cursor));
        if (!match) continue;
        const tag = match[1].toLowerCase(), attrs = attributes(match[2]);
        const at = locationAt(source, start);
        if (attrs.id) symbols.push({ kind: 'dom-id', name: attrs.id, owner: tag, ...at, endLine: locationAt(source, end).line,
            start, length: cursor - start, signature: tag + '#' + attrs.id, language: 'HTML' });
        for (const key of ['src', 'href', 'data-controller', 'data-guide-image']) if (attrs[key]) resources.push({ attribute: key, value: attrs[key], ...at });
        if (tag !== 'script' && tag !== 'style') continue;
        const closePattern = new RegExp('</' + tag + '\\s*>', 'ig'); closePattern.lastIndex = cursor;
        const close = closePattern.exec(source);
        assert(close, 'Unterminated HTML ' + tag + ' element');
        const blockAt = locationAt(source, cursor);
        const content = source.slice(cursor, close.index);
        symbols.push({ kind: tag + '-block', name: tag + '@' + at.line, owner: '', ...at,
            endLine: locationAt(source, close.index).line, start, length: closePattern.lastIndex - start,
            signature: '<' + tag + '>', language: 'HTML' });
        if (tag === 'script' && !attrs.src && (!attrs.type || /^(module|(?:text|application)\/javascript)$/i.test(attrs.type))) {
            symbols.push(...javascriptSymbols(content, acorn, { lineOffset: blockAt.line - 1,
                columnOffset: blockAt.column - 1, sourceType: attrs.type === 'module' ? 'module' : 'script', owner: 'script@' + at.line }));
        }
        cursor = closePattern.lastIndex;
    }
    return { symbols, resources };
}

function flowDefinitions(root, config, inputs) {
    const document = readJson(path.join(root, config.flowFile));
    assert(document.schemaVersion === 1 && Array.isArray(document.flows), 'Unsupported flow schema');
    const paths = new Set(inputs.map(file => file.path)), seen = new Set();
    for (const flow of document.flows) {
        assert(typeof flow.id === 'string' && /^[a-z][a-z0-9-]*$/.test(flow.id) && !seen.has(flow.id), 'Invalid/duplicate flow id');
        seen.add(flow.id);
        assert(typeof flow.title === 'string' && typeof flow.summary === 'string' && flow.sources?.length > 0, 'Incomplete flow definition: ' + flow.id);
        assert(new Set(flow.sources).size === flow.sources.length, 'Duplicate flow source');
        for (const filename of flow.sources) assert(paths.has(safeRelative(filename)), 'Missing flow source: ' + filename);
    }
    assert(document.flows.length > 0, 'Empty reviewed flow set');
    const assigned = new Set(document.flows.flatMap(flow => flow.sources));
    for (const input of inputs) {
        if (!input.path.startsWith('tools/') && /\.(?:cs|[cm]?js|html)$/i.test(input.path))
            assert(assigned.has(input.path), 'Source has no reviewed flow ownership: ' + input.path);
    }
    return document.flows;
}

function reviewBindings(root, config, inputs, flows) {
    const hashes = new Map(inputs.map(file => [file.path, file.sha256]));
    const guideHash = hashes.get(config.guideFile);
    assert(guideHash, 'Missing semantic guide');
    // New files can introduce new behavior without appearing in an old flow's
    // explicit source list. Never let regeneration silently bless that change.
    const inputSetHash = hash(json(inputs.map(file => file.path)));
    return Object.fromEntries(flows.map(flow => [flow.id, {
        inputSetHash,
        definitionHash: hash(json({ flow, guideHash })),
        sourceHashes: Object.fromEntries(sorted(flow.sources).map(filename => [filename, hashes.get(filename)]))
    }]));
}

export function review(root, confirmed) {
    assert(confirmed, 'Review is deliberate: read the guide and all bound changes, then pass --confirm-reviewed');
    const config = loadConfig(root), inputs = snapshot(root, config), flows = flowDefinitions(root, config, inputs);
    writeFile(path.join(root, config.reviewFile), json({ schemaVersion: 1, flows: reviewBindings(root, config, inputs, flows) }));
}

function reviewProblems(root, config, inputs) {
    const flows = flowDefinitions(root, config, inputs);
    const expected = reviewBindings(root, config, inputs, flows);
    if (!fs.existsSync(path.join(root, config.reviewFile))) return ['Semantic review file is missing'];
    const actual = readJson(path.join(root, config.reviewFile));
    assert(actual.schemaVersion === 1 && actual.flows && typeof actual.flows === 'object', 'Invalid review schema');
    const problems = [];
    for (const id of sorted(new Set([...Object.keys(expected), ...Object.keys(actual.flows)]))) {
        if (json(expected[id]) !== json(actual.flows[id])) problems.push('Semantic review stale or missing: ' + id);
    }
    return problems;
}

function markdown(map) {
    const lines = ['# Code map', '', 'Generated syntax inventory. Start with [the reviewed change guide](CODE_GUIDE.md) and [maintenance commands](CODE_MAP_MAINTENANCE.md).', '',
        'Run `node tools/code-map.mjs check` before trusting this snapshot. A generated map is not a compiler-bound call graph or a runtime capability test.', '',
        `Scope: ${map.files.length} files; ${map.files.reduce((sum, file) => sum + file.symbols.length, 0)} syntax declarations/HTML anchors.`, '',
        'C# includes namespace/type/member/local/parameter declarations and anonymous functions. JavaScript includes functions, classes/methods, bindings and parameters. HTML records IDs, script/style blocks and resource references. Compiler-synthesized members and external Emby bundle code are not repository declarations.', '',
        'C# constant JavaScript payload coordinates point to the owning constant; decoded payload coordinates are retained in JSON. Other dashboard replacement strings are fragments owned by their C# patch methods.', '',
        '| File | Role | Entries |', '|---|---|---:|'];
    for (const file of map.files) lines.push(`| [${escapeMd(file.path)}](#${file.anchor}) | ${escapeMd(file.buildRole)} | ${file.symbols.length} |`);
    for (const file of map.files) {
        lines.push('', `<a id="${file.anchor}"></a>`, `## [${escapeMd(file.path)}](${sourceLink(file.path)})`, '',
            `${file.buildRole}; ${file.bytes} bytes${file.lines == null ? '' : '; ' + file.lines + ' lines'}.`);
        if (file.resources?.length) {
            lines.push('', 'Resource/controller references:', '');
            for (const resource of file.resources) lines.push(`- Line [${resource.line}](${sourceLink(file.path, resource.line)}): ${escapeMd(resource.attribute)} = \`${escapeMd(resource.value)}\``);
        }
        if (!file.symbols.length) continue;
        lines.push('', '<details>', `<summary>All ${file.symbols.length} declarations / anchors</summary>`, '', '| Kind | Owner / name | Source |', '|---|---|---|');
        for (const symbol of file.symbols) lines.push(`| ${escapeMd(symbol.kind)} | ${escapeMd((symbol.owner ? symbol.owner + '.' : '') + symbol.name)} | [${symbol.line}:${symbol.column}](${sourceLink(file.path, symbol.line)}) |`);
        lines.push('', '</details>');
    }
    return lines.join('\n') + '\n';
}

export function render(root, csharpFilename) {
    const config = loadConfig(root), inputs = snapshot(root, config), project = projectFacts(root, config);
    validateExcludedCompileInputs(root, config, project);
    const csharp = readJson(csharpFilename);
    assert(csharp.schemaVersion === 1 && Array.isArray(csharp.files) && csharp.engineVersion, 'Invalid C# parser result');
    const expectedPaths = inputs.filter(file => /\.cs$/i.test(file.path)).map(file => file.path);
    assert(json(sorted(csharp.files.map(file => file.path))) === json(expectedPaths), 'C# parser file set is stale/incomplete');
    const parsed = new Map(csharp.files.map(file => [file.path, file]));
    assert(parsed.size === csharp.files.length, 'Duplicate C# parser file');
    // This import is intentionally absent from check/prepare/review paths.
    const acorn = require('acorn');
    const files = inputs.map(input => {
        const source = /\.(cs|[cm]?js|html|json|md|csproj|sln)$/i.test(input.path)
            ? fs.readFileSync(path.join(root, input.path), 'utf8').replace(/^\uFEFF/, '') : null;
        let symbols = [], resources = [];
        if (/\.cs$/i.test(input.path)) {
            const parsedFile = parsed.get(input.path);
            assert(parsedFile.sha256 === input.sha256, 'C# parser source changed: ' + input.path);
            assert(Array.isArray(parsedFile.declarations), 'Missing C# declarations');
            symbols = parsedFile.declarations.map(symbol => ({ ...symbol, language: 'C#' }));
            for (const payload of parsedFile.javascriptPayloads ?? []) symbols.push(...javascriptSymbols(payload.source, acorn, { owner: payload.name, payloadLine: payload.line }));
        } else if (/\.[cm]?js$/i.test(input.path)) {
            symbols = javascriptSymbols(source, acorn, { sourceType: input.path.endsWith('.mjs') ? 'module' : input.path.endsWith('.cjs') ? 'script' : 'unambiguous' });
        } else if (/\.html$/i.test(input.path)) ({ symbols, resources } = htmlInventory(source, acorn));
        const lineCount = source == null ? null : source.split('\n').length - (source.endsWith('\n') ? 1 : 0);
        for (const symbol of symbols) assert(Number.isInteger(symbol.line) && symbol.line > 0 && symbol.line <= lineCount
            && typeof symbol.name === 'string' && symbol.name.length > 0, 'Invalid symbol location/name: ' + input.path);
        symbols.sort((a, b) => a.line - b.line || a.column - b.column || (a.kind < b.kind ? -1 : a.kind > b.kind ? 1 : 0) || a.start - b.start);
        return { ...input, anchor: 'file-' + hash(input.path).slice(0, 16), lines: lineCount,
            buildRole: buildRole(input.path, project), symbols, resources };
    });
    const map = { schemaVersion: 1, evidence: 'Syntax-extracted declarations; reviewed semantic explanations are separate and require fresh bindings',
        excludedDirectories: config.excludedDirectories, excludedFiles: [...config.excludedFiles, ...config.generatedOutputs, config.reviewFile],
        parsers: { csharp: { engine: csharp.engine, version: csharp.engineVersion, languageVersion: csharp.languageVersion },
            javascript: { engine: 'Acorn', version: acorn.version }, html: 'Quoted-tag/comment/rawtext scanner; not a DOM validator' },
        project, flows: flowDefinitions(root, config, inputs), files };
    const outputs = { 'docs/CODE_MAP.md': markdown(map), 'docs/code-map.json': json(map) };
    const manifest = { schemaVersion: 1, inputs: inputs.map(({ path, sha256 }) => ({ path, sha256 })),
        outputs: Object.entries(outputs).map(([filename, content]) => ({ path: filename, sha256: hash(content) })), parsers: map.parsers };
    outputs['docs/code-map.manifest.json'] = json(manifest);
    return outputs;
}

export function generate(root, csharpFilename, compareOnly = false) {
    const outputs = render(root, csharpFilename);
    for (const [filename, content] of Object.entries(outputs)) {
        if (compareOnly) assert(fs.existsSync(path.join(root, filename)) && fs.readFileSync(path.join(root, filename), 'utf8') === content,
            'Generated output differs: ' + filename);
        else writeFile(path.join(root, filename), content);
    }
    return Object.keys(outputs).length;
}

export function check(root) {
    const config = loadConfig(root), inputs = snapshot(root, config);
    validateExcludedCompileInputs(root, config, projectFacts(root, config));
    const manifest = readJson(path.join(root, 'docs/code-map.manifest.json'));
    assert(manifest.schemaVersion === 1 && Array.isArray(manifest.inputs) && Array.isArray(manifest.outputs), 'Invalid map manifest');
    const expected = new Map(inputs.map(file => [file.path, file.sha256]));
    const actual = new Map(manifest.inputs.map(file => [file.path, file.sha256]));
    assert(actual.size === manifest.inputs.length, 'Duplicate manifest input');
    const problems = [];
    for (const filename of sorted(new Set([...expected.keys(), ...actual.keys()]))) {
        if (!actual.has(filename)) problems.push('New input: ' + filename);
        else if (!expected.has(filename)) problems.push('Removed input: ' + filename);
        else if (expected.get(filename) !== actual.get(filename)) problems.push('Changed input: ' + filename);
    }
    assert(json(sorted(manifest.outputs.map(file => file.path))) === json(sorted(config.generatedOutputs.filter(file => !file.endsWith('.manifest.json')))), 'Incomplete generated-output hash set');
    for (const output of manifest.outputs) {
        const filename = path.join(root, safeRelative(output.path));
        if (!fs.existsSync(filename) || hash(fs.readFileSync(filename)) !== output.sha256) problems.push('Missing/changed generated output: ' + output.path);
    }
    problems.push(...reviewProblems(root, config, inputs));
    assert(problems.length === 0, problems.join('\n'));
    return { files: inputs.length, message: 'CODE_MAP_CURRENT (hash freshness and reviewed bindings; no parser/runtime execution)' };
}

function main() {
    const [command, ...args] = process.argv.slice(2);
    const allowed = new Set(['--root', '--input', '--csharp', '--confirm-reviewed']);
    const options = {};
    for (let i = 0; i < args.length; i++) {
        assert(allowed.has(args[i]) && !(args[i] in options), 'Unknown/duplicate option: ' + args[i]);
        const key = args[i];
        options[key] = key === '--confirm-reviewed' ? true : args[++i];
        assert(options[key] != null, 'Missing option value: ' + key);
    }
    const root = path.resolve(options['--root'] ?? DEFAULT_ROOT);
    if (command === 'prepare') {
        assert(options['--input'], 'prepare requires --input <private-output.json>');
        console.log('CODE_MAP_PREPARED files=' + prepare(root, path.resolve(options['--input'])));
    } else if (command === 'generate' || command === 'verify-generation') {
        assert(options['--csharp'], command + ' requires --csharp <parser-output.json>');
        console.log('CODE_MAP_' + (command === 'generate' ? 'GENERATED' : 'DETERMINISTIC') + ' outputs=' + generate(root, path.resolve(options['--csharp']), command !== 'generate'));
    } else if (command === 'check') console.log(check(root).message);
    else if (command === 'review') {
        review(root, options['--confirm-reviewed']); console.log('CODE_MAP_SEMANTICS_REVIEW_BOUND');
    } else throw new Error('Usage: node tools/code-map.mjs <prepare|generate|verify-generation|check|review> [--root repository] [--input prepared.json] [--csharp parsed.json] [--confirm-reviewed]');
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
    try { main(); }
    catch (error) { console.error('Code map failed: ' + error.message); process.exitCode = 1; }
}
