# Internal Ticket Manager

Aplicacao ASP.NET Core MVC simples para gerir tickets internos.

O repositorio serve como base de formacao e como starter app pequeno para
listar, criar, consultar e atualizar tickets de suporte ou operacoes internas.

## Estrutura do repositorio

```text
.
|-- src/                         Codigo-fonte da aplicacao
|-- tests/                       Testes automatizados
|-- docs/                        Documentacao, requisitos e material de formacao
|-- build/                       Scripts e configuracao de build
|-- deploy/                      Scripts e configuracao de deploy, se existirem
|-- scripts/                     Scripts locais de apoio ao desenvolvimento
|-- .github/                     Workflows e instrucoes para GitHub
|-- AGENTS.md                    Instrucoes para agentes de IA/codigo
|-- Directory.Build.props        Configuracao partilhada dos projetos .NET
|-- InternalTicketManager.sln    Solucao .NET
|-- LICENSE.md                   Licenca e direitos de uso
|-- README.md                    Guia principal do repositorio
|-- .gitignore                   Ficheiros excluidos do controlo de versoes
`-- .gitattributes               Regras de normalizacao de texto e binarios
```

## Organizacao de pastas

- `src/TicketManager.Web/`: aplicacao web ASP.NET Core MVC.
- `tests/TicketManager.Web.Tests/`: testes automatizados da aplicacao.
- `docs/specs/`: requisitos funcionais e especificacoes por funcionalidade.
- `docs/exercises/`: enunciados de exercicios.
- `docs/trainer/`: material de apoio ao formador.
- `docs/generated/`: documentacao tecnica gerada a partir do codigo.
- `build/`: ficheiros de build que nao pertencam diretamente a `.github/`.
- `deploy/`: ficheiros de deploy e configuracao de ambientes.

## Projetos

| Projeto | Caminho | Descricao |
| --- | --- | --- |
| `TicketManager.Web` | `src/TicketManager.Web/TicketManager.Web.csproj` | Aplicacao web MVC |
| `TicketManager.Web.Tests` | `tests/TicketManager.Web.Tests/TicketManager.Web.Tests.csproj` | Testes automatizados |

## Requisitos

- .NET 8 SDK
- SQLite e usado pela aplicacao atraves do provider Entity Framework Core

## Como usar

Restaurar dependencias:

```powershell
dotnet restore InternalTicketManager.sln
```

Executar a aplicacao:

```powershell
dotnet run --project src/TicketManager.Web
```

Em Windows, tambem pode ser usado o script local:

```powershell
.\scripts\start-app.ps1
```

Se a porta predefinida estiver ocupada:

```powershell
.\scripts\start-app.ps1 -Port 5130
```

A aplicacao cria a base de dados automaticamente e inicializa tickets de
exemplo quando a base de dados esta vazia.

Por predefinicao, a base de dados local usa o ficheiro `InternalTicketManager.db`.

## Testes

Os testes de integracao usam uma base SQLite temporaria e isolada por execucao.

```powershell
dotnet test InternalTicketManager.sln
```

## Documentacao e especificacoes

- Requisitos e specs funcionais: `docs/specs/`
- Fluxo de trabalho da formacao: `docs/00-training-flow.md`
- Standard de documentacao tecnica: `docs/documentation-standard.md`
- Guia de mensagens de commit: `docs/commit-messages.md`

Gerar documentacao tecnica:

```powershell
.\docs\generate-docs.ps1 -Open
```

## Mensagens de commit

Usar Conventional Commits:

```text
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

Tipos principais:

| Tipo | Quando usar |
| --- | --- |
| `feat` | Nova funcionalidade |
| `fix` | Correcao de uma anomalia |
| `docs` | Alteracoes apenas na documentacao |

Exemplo:

```text
docs(readme): documentar estrutura do repositorio
```

Regras completas em `docs/commit-messages.md`.

## Licenca

Este repositorio nao define uma licenca publica aberta. Ver `LICENSE.md`.
