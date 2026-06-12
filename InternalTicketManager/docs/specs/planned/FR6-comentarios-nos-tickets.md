# FR6 - Comentarios nos tickets

## Objetivo

Permitir registar contexto de acompanhamento nos tickets internos atraves de
comentarios simples, visiveis no detalhe do ticket.

## Contexto usado

- `docs/exercises/tarefa-por-fazer.md`: pedido inicial intencionalmente vago.
- `docs/00-training-flow.md`: exige spec, plano, implementacao, testes e docs.
- `AGENTS.md`: manter a app simples e evitar utilizadores, autenticacao,
  anexos, notificacoes ou workflow extra.
- `src/TicketManager.Web/Controllers/TicketsController.cs`: o detalhe do ticket
  ja e a pagina natural para acompanhar o pedido.

## Ambito

- Mostrar comentarios existentes na pagina de detalhe do ticket.
- Adicionar comentario a partir da pagina de detalhe.
- Guardar autor livre, texto do comentario e data UTC de criacao.
- Validar os campos obrigatorios e tamanhos maximos.
- Manter protecao antiforgery no formulario.

## Fora de ambito

- Utilizadores reais, autenticacao, autorizacao ou permissoes.
- Edicao, eliminacao, anexos, notificacoes ou mencoes em comentarios.
- Historico automatico de alteracoes de estado.
- Comentarios em tickets inexistentes.

## Rotas envolvidas

- `GET /Tickets/Details/{id}`: mostra os dados do ticket, a lista de
  comentarios e o formulario de novo comentario.
- `POST /Tickets/{id}/Comments`: valida e cria um comentario para o ticket.

## Comportamento esperado

- No detalhe, os comentarios aparecem por ordem cronologica, do mais antigo para
  o mais recente.
- Se nao existirem comentarios, a pagina mostra um estado vazio simples.
- O formulario pede `Autor` e `Comentario`.
- Ao submeter dados validos, a aplicacao cria o comentario com `CreatedAt` em
  UTC e redireciona para o detalhe do ticket.
- Ao submeter dados invalidos, a pagina de detalhe volta a ser apresentada com
  mensagens de validacao e sem criar comentario.
- Ao comentar um ticket inexistente, a aplicacao devolve `404 Not Found`.

## Dados envolvidos

- `TicketComment.Id`: identificador gerado pela base de dados.
- `TicketComment.TicketId`: ticket associado, obrigatorio.
- `TicketComment.AuthorName`: autor livre, obrigatorio, maximo 80 caracteres.
- `TicketComment.Body`: texto do comentario, obrigatorio, maximo 1000
  caracteres.
- `TicketComment.CreatedAt`: data/hora UTC de criacao, obrigatoria.

## Regras de negocio

- Um comentario pertence sempre a um unico ticket.
- O autor e texto nao podem estar vazios.
- A app nao tenta associar o comentario a um utilizador autenticado.
- Comentarios nao alteram prioridade, estado ou outros dados do ticket.

## Regras tecnicas

- Usar Entity Framework Core e o `AppDbContext` existente.
- Configurar limites e relacao no `OnModelCreating`.
- Carregar comentarios no detalhe com `Include`.
- Usar uma action POST dedicada com `[ValidateAntiForgeryToken]`.
- Manter XML documentation comments em novos tipos publicos e actions.

## Criterios de aceitacao

- Dado um ticket existente sem comentarios, o detalhe mostra a seccao de
  comentarios, o formulario e o estado vazio.
- Dado um ticket existente, quando submeto autor e comentario validos, sou
  redirecionado para o detalhe e o comentario fica persistido.
- Dado um ticket com comentarios, o detalhe mostra autor, texto e data local de
  cada comentario.
- Dado autor ou comentario em falta, a pagina mostra erros de validacao e nao
  persiste dados.
- Dado um ticket inexistente, `GET /Tickets/Details/{id}` e
  `POST /Tickets/{id}/Comments` devolvem `404`.
- Dado um POST sem antiforgery token, a aplicacao rejeita o pedido.

## Casos de erro

- Ticket inexistente: `404 Not Found`.
- Campos obrigatorios vazios: erros de validacao no formulario.
- Campos acima do limite: erros de validacao no formulario.
- Token antiforgery ausente ou invalido: `400 Bad Request`.

## Plano tecnico

1. Adicionar o modelo `TicketComment` e a colecao `Comments` em `Ticket`.
2. Configurar `DbSet<TicketComment>` e relacao no `AppDbContext`.
3. Atualizar `TicketsController.Details` para carregar comentarios.
4. Adicionar action `AddComment` para `POST /Tickets/{id}/Comments`.
5. Atualizar `Views/Tickets/Details.cshtml` com lista, estado vazio e formulario.
6. Adicionar estilos pequenos para comentarios.
7. Cobrir validacoes e fluxo HTTP nos testes.
8. Atualizar README e correr build/testes.

Estado atual: funcionalidade implementada no ambito desta spec.
