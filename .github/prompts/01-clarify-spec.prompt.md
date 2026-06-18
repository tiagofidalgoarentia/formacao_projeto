---
description: Create an implementation-ready feature spec from the request, project rules, docs, and existing code.
---

# Clarify Spec

Create a feature specification before coding.

Follow this workflow:

1. Read the user request and restate the target outcome in concrete product terms.
2. Read project agent rules that apply to the target area. Look for `AGENTS.md`, `agents.md`, `.agents/`, and nested rule files.
3. Search repository docs before inventing assumptions. Prefer `docs/`, `specs/`, `requirements/`, `planning/`, `architecture/`, `README*`, and domain files referenced from the agent rules.
4. Inspect existing code only enough to understand current behavior, naming, data models, routes, components, APIs, and test patterns relevant to the spec.
5. Separate facts from inferences. Mark unclear points as assumptions unless they block the spec.
6. Ask only for decisions that materially change product behavior or technical approach. Otherwise proceed with explicit assumptions.

Use this output format unless the repository already has a stronger local template:

```markdown
# <Feature Name>

## Goal
<What user or business outcome this feature must deliver.>

## Context Used
- <Relevant agent rule, doc, code area, or existing behavior.>

## In Scope
- <Behavior included in this implementation.>

## Out Of Scope
- <Related work intentionally excluded.>

## User Experience / Behavior
- <Concrete behavior, states, interactions, validations, permissions, and errors.>

## Data And Integration
- <Models, API contracts, storage, migrations, external systems, or none.>

## Acceptance Criteria
- <Observable pass/fail criteria.>

## Edge Cases
- <Empty, loading, error, invalid input, authorization, concurrency, responsive states.>

## Implementation Notes
- <Likely files, components, services, tests, risks, and sequencing.>

## Test Plan
- <Unit/integration/e2e/manual checks expected.>

## Assumptions
- <Assumptions made because docs or request were incomplete.>
```

Make the spec specific enough that another agent can implement it without repeating product discovery.
