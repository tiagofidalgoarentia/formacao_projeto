---
description: Review and update repository-facing documentation after code changes, especially README.md and GitHub Copilot instructions.
---

# Update Repo Docs

Review repository-facing documentation for the current change.

Follow this workflow:

1. Read `AGENTS.md`, `.github/copilot-instructions.md`, `README.md`, and any docs near the changed files before editing.
2. Inspect the actual code and tests touched by the change. Do not update documentation from assumptions alone.
3. Decide whether documentation needs a change:
   - Update `README.md` when setup, usage, repository structure, commands, features, or project behavior visible to maintainers changes.
   - Update docs under `docs/` when specs, technical documentation, training material, or commit/documentation standards change.
   - Update `.github/copilot-instructions.md` when GitHub Copilot should consistently know a repository rule in future chats.
   - Leave documentation unchanged when the code change is internal and does not alter documented behavior or workflows.
4. Keep documentation concise and product-facing. Describe the application as it exists, not as a temporary delivery task.
5. Preserve the current Portuguese documentation style unless the surrounding file is already in English.
6. If public C# domain types, enums, controllers, or data access types changed, verify XML documentation comments remain useful and consistent with `docs/documentation-standard.md`.
7. After code changes, run `dotnet build` and `dotnet test` when practical. If documentation only changed, run no .NET checks unless needed.
8. Finish by stating whether README/docs were updated or deliberately left unchanged, and why.

Use `.github/copilot-instructions.md` for stable rules that Copilot should always apply in this repository. Do not put one-off task details there.
