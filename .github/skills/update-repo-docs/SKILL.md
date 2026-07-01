---
name: update-repo-docs
description: Usa esta skill para rever e melhorar comentarios de codigo e documentacao XML apos alteracoes, especialmente em tipos publicos, controllers, endpoints e acesso a dados.
---

# Melhorar comentarios de codigo

Reve os comentarios de codigo e a documentacao XML para a alteracao atual.

## Fluxo de trabalho

1. Le `AGENTS.md` e `docs/documentation-standard.md` antes de editar comentarios.
2. Inspeciona o codigo alterado e os tipos publicos proximos. Nao atualizes comentarios apenas com base em pressupostos.
3. Verifica se comentarios XML existem e continuam uteis em:
   - Classes de dominio publicas.
   - Enums publicos.
   - Controllers e actions publicas.
   - Endpoints e contratos de API.
   - Tipos de acesso a dados e repositorios publicos.
4. Melhora comentarios que estejam desatualizados, vagos, enganadores ou que apenas repitam o nome do membro.
5. Acrescenta `<summary>`, `<param>`, `<returns>` e metadados de resposta quando forem uteis para endpoints publicos.
6. Mantem os comentarios curtos, concretos e orientados ao comportamento observavel.
7. Preserva o idioma e estilo do ficheiro envolvente. Se o codigo ja estiver em ingles, comenta em ingles; se estiver em portugues, comenta em portugues.
8. Nao acrescentes comentarios a codigo privado ou obvio salvo se explicarem uma decisao, regra de negocio ou caso-limite que nao seja evidente pelo codigo.
9. Apos alteracoes de codigo, corre `dotnet build` quando for pratico para validar documentacao XML e compilacao.
10. Termina indicando que comentarios foram melhorados e se houve validacao.

## Regras

- Prefere comentarios que expliquem intencao, comportamento publico e restricoes relevantes.
- Evita comentarios narrativos que descrevam linha a linha aquilo que o codigo ja mostra.
- Nao atualizes `README.md`, `.github/copilot-instructions.md` ou documentos de produto salvo se o utilizador pedir explicitamente.
