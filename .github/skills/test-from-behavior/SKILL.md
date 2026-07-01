---
name: test-from-behavior
description: Usa esta skill para desenhar, adicionar ou melhorar testes a partir de comportamento, relatorio de bug, especificacao, criterios de aceitacao ou codigo alterado.
---

# Testar a partir do comportamento

Cria ou melhora testes para o comportamento pedido.

## Fluxo de trabalho

1. Le a fonte do comportamento: pedido do utilizador, relatorio de bug, especificacao, criterios de aceitacao ou diff.
2. Le as regras de agentes do projeto e a orientacao de testes existente.
3. Inspeciona testes proximos e utilitarios de teste antes de escolher um padrao.
4. Escolhe o nivel de teste mais baixo que de confianca:
   - Testes unitarios para logica pura, validacao, formatacao, reducers e servicos isolados.
   - Testes de integracao para componentes, handlers de API, comportamento de base de dados, routing e contratos entre modulos.
   - Testes end-to-end para jornadas criticas de utilizador, permissoes, comportamento no browser e regressoes que testes unitarios nao conseguem representar.
5. Adiciona testes focados no comportamento esperado e em casos-limite importantes.
6. Corre o comando de testes relevante. Expande para verificacoes mais amplas quando a area alterada for partilhada ou arriscada.

## Desenho dos testes

- Nomeia testes pelo comportamento observavel.
- Prefere fixtures realistas a mocks excessivos de detalhes internos.
- Evita assercoes frageis ligadas a detalhes de implementacao, markup gerado, temporizacao ou ordenacao incidental.
- Em correcoes de bugs, adiciona um teste de regressao sempre que for viavel.
- Inclui caminhos negativos: entrada invalida, dados em falta, permissao negada, falha de rede ou servico, resultado vazio e valores-limite.
- Nao adicionas testes de snapshot salvo se o repositorio ja depender deles e o snapshot for realmente util.

## Resposta final

Termina com os testes adicionados ou alterados, comandos executados, comportamento coberto e risco residual, se existir.
