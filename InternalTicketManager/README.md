# Internal Ticket Manager

Aplicacao ASP.NET Core MVC minima para uma formacao de desenvolvimento assistido por IA.

## Objetivo

A aplicacao base gere tickets internos e deixa os comentarios nos tickets por implementar de forma intencional. Os participantes devem definir primeiro a funcionalidade em falta antes de a desenvolver.

## Ambito da base

Implementado:

- Listar tickets
- Criar ticket
- Ver detalhe de ticket
- Editar estado de ticket
- Adicionar comentarios aos tickets
- Criar 5 tickets de exemplo

Nao implementado:

- Utilizadores
- Autenticacao
- Autorizacao
- Categorias
- Anexos
- Notificacoes
- Dashboard

## Stack tecnico

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server LocalDB por defeito

## Requisitos

- .NET 8 SDK
- SQL Server LocalDB ou outra instancia de SQL Server

## Executar

A partir da raiz do repositorio:

```powershell
cd InternalTicketManager
dotnet restore
dotnet run --project src/TicketManager.Web
```

Abrir o URL indicado pelo `dotnet run`.

A aplicacao cria a base de dados automaticamente com `EnsureCreated()` e cria tickets de exemplo se a base de dados estiver vazia.

## Comentarios nos tickets

Na pagina de detalhe de cada ticket, a seccao `Comentarios` permite registar
notas de acompanhamento com autor livre e texto do comentario.

Os comentarios ficam associados ao ticket, aparecem por ordem cronologica e nao
alteram prioridade, estado ou outros dados do pedido.

## Testes

Os testes de integracao correm contra SQL Server. Indicar a connection string de teste atraves de `ConnectionStrings__TestConnection`.

Exemplo com SQL Server disponivel em `localhost`:

```powershell
$env:ConnectionStrings__TestConnection="Server=localhost,1433;Database=master;User Id=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True"
dotnet test
```

Se nao existir SQL Server local, pode ser usada a instancia de teste definida em `docker-compose.yml`:

```powershell
docker compose up -d sqlserver
$env:ConnectionStrings__TestConnection="Server=localhost,1433;Database=master;User Id=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True"
dotnet test
```

A suite de testes cria uma base de dados unica por execucao e elimina-a no fim.

## Pipeline de CI

A pipeline de CI esta definida em:

```text
../.github/workflows/ci.yml
```

A pipeline corre automaticamente quando ha:

- `push`
- `pull_request`

Tambem pode ser executada manualmente no GitHub:

1. Abrir o repositorio no GitHub.
2. Ir a `Actions`.
3. Escolher a workflow `CI`.
4. Clicar em `Run workflow`.

A pipeline executa os seguintes passos:

- restaura os pacotes NuGet;
- corre o linter de formatacao com `dotnet format`;
- faz type checking e analise estatica atraves de `dotnet build`;
- falha a build se existirem warnings do compilador ou dos analisadores;
- corre os testes automatizados contra SQL Server;
- confirma que a aplicacao consegue ser publicada;
- guarda os resultados dos testes como artefacto.

As regras partilhadas da build estao em:

```text
Directory.Build.props
```

Este ficheiro ativa analise de codigo, regras de estilo em build e `TreatWarningsAsErrors`, para a CI bloquear codigo com warnings.

Para reproduzir localmente a parte principal da CI:

```powershell
cd InternalTicketManager
dotnet restore InternalTicketManager.sln
dotnet format InternalTicketManager.sln --verify-no-changes --no-restore
dotnet build InternalTicketManager.sln --configuration Release --no-restore
dotnet test InternalTicketManager.sln --configuration Release --no-build
dotnet publish src/TicketManager.Web/TicketManager.Web.csproj --configuration Release --no-build
```

Nota: para correr os testes localmente, e necessario ter SQL Server disponivel e configurar `ConnectionStrings__TestConnection`, como descrito na seccao de testes.

## Configurar SQL Server

A connection string por defeito esta em:

```text
src/TicketManager.Web/appsettings.json
```

Valor por defeito:

```text
Server=(localdb)\mssqllocaldb;Database=InternalTicketManager;Trusted_Connection=True;MultipleActiveResultSets=true
```

Alterar este valor se os participantes usarem outra instancia de SQL Server.

## Fluxo de trabalho da formacao

O fluxo principal esta documentado em:

```text
docs/00-training-flow.md
```

Sequencia esperada:

1. Iniciar o projeto e rever contexto.
2. Fazer ou completar a spec funcional.
3. Pedir revisao da spec.
4. Pedir plano tecnico pequeno e verificavel.
5. Implementar por passos pequenos.
6. Correr build, testes e teste manual quando aplicavel.
7. Atualizar documentacao.
8. Rever diff e fazer commit.
9. Verificar a CI depois do push ou pull request.

Para o exercicio principal, completar primeiro:

```text
docs/exercises/tarefa-por-fazer.md
```

Depois transformar esse pedido na spec:

```text
docs/specs/planned/FR6-comentarios-nos-tickets.md
```

Checklist do formador:

```text
docs/trainer/checklist.md
```
