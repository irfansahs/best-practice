# Best Practice Project

Modular monolith with .NET 10 backend (DDD/CQRS), React frontend, and Expo mobile.

## Prerequisites

- Docker Desktop (recommended for full stack)
- Or: .NET 10 SDK + Node.js 22+ for local IDE debugging
- Cursor/VS Code — open **`App.code-workspace`**, not a single folder

## Quick Start

### Full dev stack in Docker (hot reload)

SQL Server, API (`dotnet watch`), Vite HMR, and Expo Web in one command:

```bash
docker compose --profile dev up -d --build
```

| Service     | URL |
|-------------|-----|
| API         | `http://localhost:5202` |
| Frontend    | `http://localhost:5173` |
| Mobile Web  | `http://localhost:8081` |
| SQL Server  | `127.0.0.1,14333` |
| Scalar      | `http://localhost:5202/scalar/v1` (Development) |

Default admin (seeded): `admin@local.dev` / `ChangeMe123!`

**Note:** `mobile-dev` is Expo **Web** in the browser. Android emulator / Expo Go still use local Metro: `cd mobile && npx expo start --android` (API at `http://10.0.2.2:5202/api/v1`).

### IDE API debugging (SQL in Docker, API local)

```bash
docker compose up -d
dotnet run --project backend/src/Api --launch-profile http
cd frontend && npm install && npm run dev   # optional
```

### Production-like preview in Docker

```bash
docker compose --profile full up -d --build
```

Compiled API + Nginx frontend (same ports: 5202, 5173).

### SQL Server only

```bash
docker compose up -d
```

## Solution Layout

```
backend/src/SharedKernel, Domain, Application, Infrastructure, Api
backend/tests/Domain.UnitTests, Application.UnitTests, Api.IntegrationTests, ArchitectureTests
frontend/
mobile/
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
