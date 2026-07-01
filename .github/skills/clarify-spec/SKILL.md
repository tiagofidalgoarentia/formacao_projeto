---
name: clarify-spec
description: Usa esta skill para criar uma especificacao de funcionalidade pronta para implementacao a partir do pedido, regras do projeto, documentacao e codigo existente, antes de escrever codigo.
---

# Clarificar Especificacao

Cria uma especificacao de funcionalidade antes da implementacao.

## Fluxo de trabalho

1. Le o pedido do utilizador e reformula o resultado pretendido em termos concretos de produto.
2. Le as regras de agentes do projeto aplicaveis a area afetada. Procura `AGENTS.md`, `agents.md`, `.agents/` e ficheiros de regras aninhados.
3. Pesquisa a documentacao do repositorio antes de inventar pressupostos. Da prioridade a `docs/`, `specs/`, `requirements/`, `planning/`, `architecture/`, `README*` e ficheiros de dominio referidos nas regras de agentes.
4. Inspeciona apenas o codigo necessario para perceber comportamento atual, nomes, modelos de dados, rotas, componentes, APIs e padroes de teste relevantes para a especificacao.
5. Separa factos de inferencias. Marca pontos pouco claros como pressupostos, exceto quando bloqueiam a especificacao.
6. Faz perguntas apenas sobre decisoes que alterem materialmente o comportamento do produto ou a abordagem tecnica. Caso contrario, avanca com pressupostos explicitos.

## Formato de saida obrigatorio

Usa sempre estes quatro pontos como estrutura principal da resposta. Podes acrescentar `Perguntas em aberto`, `Pressupostos` ou `Plano de testes` apenas quando forem necessarios, mas nao substituas estes quatro pontos.

```markdown
# <Nome da funcionalidade>

## Comportamento
O que o utilizador deve conseguir fazer.

- <Comportamento concreto esperado.>
- Ex: adicionar comentarios no detalhe do ticket.

## Dados
Que informacao existe ou tem de existir.

- <Campos, modelos, armazenamento, API ou informacao necessaria.>
- Ex: autor, texto e data.

## Regras
Que condicoes ou restricoes tem de ser respeitadas.

- <Validacoes, limites, permissoes, erros e casos-limite relevantes.>
- Ex: comentarios vazios nao sao permitidos.

## Criterios de aceitacao
Como sabemos que a feature ficou correta.

- <Criterio observavel e testavel.>
- Ex: ao guardar, o comentario aparece no detalhe.
```

Se precisares de informacao adicional, acrescenta no fim:

```markdown
## Perguntas em aberto
- <Pergunta que bloqueia comportamento ou decisao tecnica.>

## Pressupostos
- <Pressuposto razoavel para avancar sem resposta.>

## Plano de testes
- <Teste unitario, integracao, e2e ou verificacao manual esperada.>
```

Torna a especificacao suficientemente concreta para que outro agente a consiga implementar sem repetir a descoberta de produto.
