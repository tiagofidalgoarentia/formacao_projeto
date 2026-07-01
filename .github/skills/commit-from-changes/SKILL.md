---
name: commit-from-changes
description: Usa esta skill para gerar uma mensagem de Conventional Commit a partir das alteracoes atuais de Git/GitHub e, opcionalmente, criar o commit.
---

# Commit a partir das alteracoes

Gera uma mensagem de commit a partir das alteracoes atuais de Git, no formato do repositorio.

## Fluxo de trabalho

1. Le `docs/commit-messages.md` quando existir. Trata-o como a fonte local de verdade.
2. Inspeciona as alteracoes antes de escrever a mensagem:
   - Corre `git status --short`.
   - Da prioridade ao diff em stage com `git diff --cached --stat` e `git diff --cached`.
   - Se nada estiver em stage, inspeciona alteracoes fora de stage com `git diff --stat` e `git diff`.
   - Inclui ficheiros nao seguidos lendo os respetivos caminhos e conteudos relevantes.
3. Identifica a intencao principal da alteracao. Se existirem varias intencoes nao relacionadas, recomenda commits separados em vez de forcar uma mensagem vaga.
4. Escolhe o tipo de commit:
   - `feat`: nova funcionalidade.
   - `fix`: correcao de uma anomalia.
   - `docs`: alteracoes apenas na documentacao.
5. Escolhe um scope opcional apenas quando tornar o commit mais facil de localizar, como `readme`, `tickets`, `ci`, `docs` ou `tests`.
6. Escreve o assunto exatamente neste formato:

```text
<type>[optional scope]: <description>
```

7. Mantem a descricao curta, concreta e em portugues quando a documentacao do repositorio estiver em portugues.
8. Acrescenta corpo apenas quando for util para explicar contexto, impacto ou decisao. Mantem-no conciso e separado do assunto por uma linha em branco.
9. Acrescenta rodapes apenas quando aplicavel:
   - `BREAKING CHANGE: <description>` para quebras de compatibilidade.
   - `Resolve: #123` para referencias a issues.
10. Se o utilizador pedir para criar o commit, coloca em stage apenas os ficheiros pretendidos e depois corre `git commit` com a mensagem gerada. Nao coloques ficheiros nao relacionados em stage.

## Saida

Quando estiveres apenas a gerar a mensagem, devolve apenas a mensagem de commit proposta num bloco `text`, seguida de uma nota curta se os ficheiros parecerem nao relacionados ou arriscados.

## Regras

Nao inventes numeros de issues, quebras de compatibilidade ou scopes. Nao facas amend, reset, squash, rebase ou push salvo se o utilizador o pedir explicitamente.
