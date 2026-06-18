---
description: Implement a feature from an existing spec, ticket, requirements document, or acceptance criteria.
---

# Implement From Spec

Implement the requested feature from the provided or referenced spec.

Follow this workflow:

1. Locate and read the spec, ticket, or acceptance criteria. If the user pasted the spec, treat that as the source of truth.
2. Read project agent rules that apply to the touched area. Look for `AGENTS.md`, `agents.md`, `.agents/`, and nested rule files.
3. Search for existing implementations of similar behavior before adding new patterns.
4. Identify the smallest coherent implementation path: data/model changes, API/service changes, UI changes, tests, and docs.
5. If a decision is blocking or the spec conflicts with project rules, ask the user. Otherwise proceed using explicit assumptions.
6. Implement incrementally and keep edits scoped to the feature.
7. Add or update tests based on behavior and risk.
8. Run the most relevant checks first, then broader checks when practical.
9. If UI changes are included, do a practical browser check when feasible and note any responsive states verified.

Rules:

- Treat project agent rules as policy and the spec as feature intent.
- Prefer existing local helpers, components, API clients, validation patterns, test utilities, and folder conventions.
- Do not turn a narrow feature into a broad refactor.
- Preserve unrelated user changes in the worktree.
- Test behavior, not implementation details.
- Cover acceptance criteria and at least one meaningful failure or edge case when applicable.

Finish with a concise summary of changed behavior, files touched, checks run, and any remaining risk.
