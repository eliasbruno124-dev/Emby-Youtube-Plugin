# Code-map maintenance

## Daily preflight: Node only

From the repository root:

```sh
node tools/code-map.mjs check
```

Requires Node 18 or newer, not .NET, Acorn, Git, network access or a running Emby instance. It is read-only. It compares the current file set (including relevant untracked additions), content hashes, tooling/configuration inputs, generated-output hashes and separately reviewed semantic bindings. `CODE_MAP_CURRENT` means those stored facts are current; it does not mean a new parser run or a playback test occurred.

After a successful preflight, search the per-file Markdown without loading the complete inventory, for example `rg -n 'TrySendPendingResumeSeekAsync|YouTubeSessionCommandRouter' docs/CODE_MAP.md` (or an editor's text search). Each result links to its current source line; use the JSON for structured queries.

Scope is the repository filesystem, excluding directory names listed in [the configuration](../tools/code-map.config.json): `.git`, `.unlazy`, `.vs`, `bin`, `obj`, `node_modules`, `scripts`, `tests`. Generated outputs and the separate review binding file are not self-hashed inputs. Source additions anywhere else are detected even before `git add`. Excluded C# files outside standard build/VCS directories must also be proven excluded by an explicit plugin Compile Remove; otherwise checking fails instead of silently hiding a plugin input. Symlinks and non-regular inventory inputs fail closed. Do not put credentials, deployment data or live logs in this source repository.

## Full regeneration

Development prerequisites: .NET SDK 8 (with its bundled Roslyn compiler), Node 18+, and Acorn 8.8.1 resolvable by Node from `tools/`. `tools/package.json` declares the development-only parser dependency; an existing system installation is sufficient. No new NuGet package is needed for the Roslyn project. The plugin does not reference or execute this tool. Its project excludes tool/private-test C# from the plugin Compile glob.

Use an existing SDK environment; these commands do not start containers or install dependencies:

```sh
dotnet build tools/CodeMap/CodeMap.csproj -c Release
node tools/code-map.mjs prepare --input .unlazy/code-map/csharp-input.json
dotnet tools/CodeMap/bin/Release/net8.0/CodeMap.dll . .unlazy/code-map/csharp-input.json .unlazy/code-map/csharp-parsed.json
node tools/code-map.mjs generate --csharp .unlazy/code-map/csharp-parsed.json
node tools/code-map.mjs verify-generation --csharp .unlazy/code-map/csharp-parsed.json
```

When using a separately managed SDK environment, give the parser the repository/input/output paths as seen inside that environment. No host-specific path or container name is embedded in the tools. The prepared C# input contains only relative filenames and SHA-256 hashes. The parser refuses changed inputs; generation refuses missing/extra parser files and changed C# hashes. Re-run prepare/parser after a C# change.

`generate` writes only `docs/CODE_MAP.md`, `docs/code-map.json` and `docs/code-map.manifest.json`; it never updates semantic approval. `verify-generation` runs the full Node/Acorn rendering path and compares exact output bytes instead of writing. For a complete fresh-parser check, rerun the preceding Roslyn parse first. Parser versions are recorded, so changing the SDK/Acorn version can intentionally change outputs.

## Deliberate semantic review

Read [CODE_GUIDE.md](CODE_GUIDE.md), [flow definitions](code-map.flows.json), and the changed source files bound to those flows. Correct explanations when needed. Only after that review:

```sh
node tools/code-map.mjs review --confirm-reviewed
node tools/code-map.mjs check
```

This binds every flow to its definition, the complete guide, source hashes and the complete inventory path set. Any added/removed input therefore requires review even if it was not in an old flow's explicit source list. It is an explicit reviewer attestation, not an automated semantic proof or cryptographic signature. Never run it just to turn a failing check green. Normal generation cannot silently bless prose. If guide/flow text changed, regenerate mechanical outputs as well because those files are inventory inputs. The quick preflight rejects pending or stale review even immediately after successful generation.

## Coverage and limitations

- Roslyn inventories source declarations, including namespaces, partial/nested types, records, members/accessors, explicit interface methods, constructors, operators, enum members, aliases, named tuple elements, local functions/variables, parameters, patterns, query bindings and anonymous functions. Entries retain owner, syntax kind, source span and location. It is not an Emby-linked semantic compilation or whole-program call graph; compiler-synthesized members are not source declarations. Inactive conditional C# text currently fails closed instead of disappearing from the index.
- Acorn inventories JS functions/arrows, classes/methods, bindings and parameters in JS files and executable inline HTML scripts. A quoted-tag/comment/rawtext scanner locates HTML script/style blocks, IDs and resource references; it is not a full DOM validator or CSS parser. Parse errors fail the generator.
- The decoded C# `PlayerPatchHelpers`, `IframeTeardownHelper` and complete `legacyAbortHelper` matching pattern are also parsed as JavaScript. Their entries link to the owning C# constants and retain decoded-payload coordinates; a matching pattern is not claimed to be injected code. Other dashboard replacement strings are partial external-bundle snippets, represented by their C# patch-method ownership, not a fabricated standalone program.
- Build roles describe this project's SDK default Compile/explicit Remove and simple resource Include declarations, not full MSBuild evaluation. Conditional/custom Compile inputs, imports, implicit ancestor `Directory.Build.props`/`Directory.Build.targets` filenames, expressions in Compile Remove or unsupported default-item rules fail visibly and require evaluator updates. External MSBuild resource expressions (embedded Harmony) are listed without resolving/deploying packages.
- Generated links are relative repository links with current line anchors. The JSON contains the full index; the Markdown groups every declaration under collapsible per-file tables. The concise guide explains cross-file behavior and recommended change locations.

No hooks, watchers, background daemon, runtime plugin dependency or production API call is installed. Keep the maintenance rule in [AGENTS.md](../AGENTS.md) with the source so future work has an explicit preflight.
