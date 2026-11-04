# Repository maintenance

- Before relying on architecture documentation, run `node tools/code-map.mjs check` from the repository root. A failure is visible stale/pending evidence, not permission to ignore the map.
- After source, resource or tooling changes, follow `docs/CODE_MAP_MAINTENANCE.md`: fresh Roslyn parse, deterministic generation and exact regeneration comparison. Keep `docs/CODE_MAP.md`, `docs/code-map.json` and their manifest current.
- Read and correct the affected explanations in `docs/CODE_GUIDE.md` and flow definitions. Only an actual source review authorizes `review --confirm-reviewed`; ordinary generation must never refresh semantic bindings automatically.
- Keep syntax-extracted facts distinct from reviewed architecture and observed runtime behavior. A successful build or command send does not establish support in every Emby/native client.
- Preserve tool/private-test C# exclusions from the plugin build. Code-map tooling is development-only. Do not install hooks, background watchers or a runtime map dependency.
- Do not add live configuration, credentials, tokens, logs or environment-specific deployment identifiers to the map or documentation. Do not edit README as an incidental map-maintenance step.
