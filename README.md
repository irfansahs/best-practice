# Best Practice Project

Modular monolith with .NET 10 backend (DDD/CQRS) and React frontend.

## Prerequisites

- .NET 10 SDK
- Node.js 22+
- Docker Desktop (SQL Server)
- Cursor/VS Code — open **`App.code-workspace`**, not a single folder

## Quick Start

### Hybrid (recommended for API debugging)

UI runs in Docker; API runs locally so you can attach a debugger.

```bash
docker compose --profile hybrid up -d --build
dotnet run --project backend/src/Api --launch-profile http
```

Open `http://localhost:5173` — the browser calls the local API at `http://localhost:5202`.

### Full stack in Docker

```bash
docker compose --profile full up -d --build
```

### SQL Server only + local dev

```bash
docker compose up -d
dotnet run --project backend/src/Api --launch-profile http
cd frontend && npm install && npm run dev
```

| Service    | URL |
|------------|-----|
| API        | `http://localhost:5202` |
| Frontend   | `http://localhost:5173` |
| SQL Server | `127.0.0.1,14333` |
| Scalar     | `http://localhost:5202/scalar/v1` (Development) |

Default admin (seeded):

- Email: `admin@local.dev`
- Password: `ChangeMe123!`

## Solution Layout

```
backend/src/SharedKernel, Domain, Application, Infrastructure, Api
backend/tests/Domain.UnitTests, Application.UnitTests, Api.IntegrationTests, ArchitectureTests
frontend/
```

## Commands

```bash
dotnet build backend/App.slnx
dotnet test backend/App.slnx
dotnet ef migrations add <Name> --project backend/src/Infrastructure --startup-project backend/src/Api
dotnet ef migrations has-pending-model-changes --project backend/src/Infrastructure --startup-project backend/src/Api
cd frontend && npm run dev && npm run build && npm run lint && npm run test
```

## Architecture

See [AGENTS.md](AGENTS.md) and [docs/01-architecture.md](docs/01-architecture.md).
