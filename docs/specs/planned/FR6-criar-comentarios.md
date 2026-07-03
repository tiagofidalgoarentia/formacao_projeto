# FR6 - Criar comentarios no detalhe do ticket

## Comportamento

Permitir adicionar comentarios no detalhe de um ticket para registar acompanhamento.

- O detalhe de ticket mostra a lista de comentarios existentes.
- O detalhe de ticket mostra um formulario para criar novo comentario na mesma pagina.
- Ao submeter comentario valido, a aplicacao guarda o comentario e redireciona para o mesmo detalhe (`/Tickets/Details/{id}`).
- A criacao de comentario nao altera automaticamente o estado do ticket.
- Quando o ticket esta `Closed`, o detalhe deve mostrar um botao proprio para reabrir.
- Quando o ticket esta `Open` ou `InProgress`, o detalhe deve mostrar um botao proprio para fechar.
- No contexto do detalhe com comentarios, nao deve existir edicao livre do estado; as unicas acoes de estado permitidas sao reabrir ticket fechado e fechar ticket em `Open` ou `InProgress`.

## Dados

Informacao necessaria para suportar comentarios e as acoes de estado no detalhe.

- Entidade `TicketComment`:
- `Id` (int, identificador).
- `TicketId` (int, referencia obrigatoria para `Ticket`).
- `AuthorName` (string, obrigatorio, 2 a 80 caracteres).
- `Content` (string, obrigatorio, 1 a 1000 caracteres).
- `CreatedAt` (DateTime UTC, preenchido automaticamente).
- Relacao `Ticket` 1:N `Comments`.
- Rotas previstas:
- `GET /Tickets/Details/{id}` para visualizar ticket, comentarios e acoes disponiveis.
- `POST /Tickets/AddComment/{id}` para criar comentario.
- `POST /Tickets/Reopen/{id}` para reabrir ticket fechado por pedido explicito do utilizador.
- `POST /Tickets/Close/{id}` para fechar ticket em `Open` ou `InProgress` por pedido explicito do utilizador.

## Regras

Regras funcionais e tecnicas da introducao de comentarios.

- `AuthorName` e obrigatorio.
- `AuthorName` com menos de 2 ou mais de 80 caracteres e invalido.
- `Content` e obrigatorio.
- `Content` com apenas espacos em branco e invalido.
- `Content` com mais de 1000 caracteres e invalido.
- Nao e permitido criar comentario para ticket inexistente.
- A criacao de comentario usa `POST` com token antiforgery valido.
- `CreatedAt` e guardado em UTC.
- Comentarios no detalhe sao listados por ordem cronologica ascendente (mais antigo para mais recente).
- Adicionar comentario num ticket `Closed` mantem o ticket em `Closed`.
- Reabrir ticket fechado so acontece por acao explicita do utilizador no botao de reabrir.
- Reabrir ticket que nao existe devolve `404 Not Found`.
- Fechar ticket em `Open` ou `InProgress` so acontece por acao explicita do utilizador no botao de fechar.
- Fechar ticket que nao existe devolve `404 Not Found`.

## Criterios de aceitacao

- Dado um ticket existente, quando o utilizador abre `/Tickets/Details/{id}`, entao ve comentarios existentes e formulario para novo comentario.
- Dado um formulario valido, quando o utilizador submete comentario, entao o comentario e guardado no ticket correto e aparece no detalhe apos redirecionamento.
- Dado um ticket com varios comentarios, quando o detalhe e apresentado, entao os comentarios surgem por ordem cronologica ascendente.
- Dado `AuthorName` vazio ou fora dos limites, quando o formulario e submetido, entao o comentario nao e criado e sao apresentadas mensagens de validacao.
- Dado `Content` vazio, so com espacos ou acima do limite, quando o formulario e submetido, entao o comentario nao e criado e sao apresentadas mensagens de validacao.
- Dado um pedido sem token antiforgery valido para criar comentario, quando o servidor recebe o pedido, entao devolve `400 Bad Request` e nao grava comentario.
- Dado um `id` de ticket inexistente para criar comentario, quando o servidor recebe o pedido, entao devolve `404 Not Found`.
- Dado um ticket em estado `Closed`, quando o utilizador cria comentario valido, entao o estado do ticket permanece `Closed`.
- Dado um ticket em estado `Closed`, quando o utilizador clica no botao de reabrir, entao o estado muda para `Open` e o detalhe e novamente apresentado.
- Dado um ticket em estado `Open`, quando o utilizador clica no botao de fechar, entao o estado muda para `Closed` e o detalhe e novamente apresentado.
- Dado um ticket em estado `InProgress`, quando o utilizador clica no botao de fechar, entao o estado muda para `Closed` e o detalhe e novamente apresentado.

## Pressupostos

- Nao existe autenticacao no ambito atual; por isso o autor do comentario e introduzido manualmente em `AuthorName`.