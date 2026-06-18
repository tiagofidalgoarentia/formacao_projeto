---
description: Generate a Conventional Commit message from current Git/GitHub changes and optionally create the commit.
---

# Commit From Changes

Generate a commit message from current Git changes in the repository format.

Follow this workflow:

1. Read `docs/commit-messages.md` when it exists. Treat it as the local source of truth.
2. Inspect changes before writing the message:
   - Run `git status --short`.
   - Prefer staged diff with `git diff --cached --stat` and `git diff --cached`.
   - If nothing is staged, inspect unstaged changes with `git diff --stat` and `git diff`.
   - Include untracked files by reading their paths and relevant contents.
3. Identify the main intent of the change. If multiple unrelated intents exist, recommend separate commits instead of forcing one vague message.
4. Choose the commit type:
   - `feat`: nova funcionalidade.
   - `fix`: correcao de uma anomalia.
   - `docs`: alteracoes apenas na documentacao.
5. Choose an optional scope only when it makes the commit easier to locate, such as `readme`, `tickets`, `ci`, `docs`, or `tests`.
6. Write the subject in this exact format:

```text
<type>[optional scope]: <description>
```

7. Keep the description short, concrete, and in Portuguese when the repository documentation is Portuguese.
8. Add a body only when useful to explain context, impact, or the decision. Keep it concise and separated from the subject by one blank line.
9. Add footers only when applicable:
   - `BREAKING CHANGE: <description>` for compatibility breaks.
   - `Resolve: #123` for issue references.
10. If the user asks to create the commit, stage only the intended files, then run `git commit` with the generated message. Do not stage unrelated files.

When only generating the message, output just the proposed commit message in a fenced `text` block, followed by a short note if files appear unrelated or risky.

Do not invent issue numbers, breaking changes, or scopes. Do not amend, reset, squash, rebase, or push unless the user explicitly asks.
