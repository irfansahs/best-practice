# Architecture Overview

## Layers

| Layer | Project | Responsibility |
| --- | --- | --- |
| SharedKernel | `SharedKernel` | Primitives, Result, guards |
| Domain | `Domain` | Aggregates, value objects, domain events |
| Application | `Application` | CQRS handlers, validators, behaviors |
| Infrastructure | `Infrastructure` | EF Core, auth, cache, logging, localization |
| Api | `Api` | Minimal APIs, middleware, composition root |

Dependency direction: `SharedKernel <- Domain <- Application <- Infrastructure <- Api`

## CQRS

Custom `IDispatcher` with pipeline behaviors:

`Logging -> Authorization -> Validation -> Caching -> Transaction -> Performance`

## Data Access

- Writes: aggregate + repository + `IUnitOfWork.SaveChangesAsync`
- Reads: `IAppDbContext` with `AsNoTracking` projections
- `AppDbContext : DbContext, IAppDbContext, IUnitOfWork`

## Cross-Cutting

- Cache: `ICacheService` over HybridCache (L1 memory)
- Localization: DB-backed translations + aggregate translations
- Logging: Serilog to SQL Server + audit table
- Auth: custom JWT + refresh token rotation + permissions

## Frontend

Vite + React + Redux Toolkit (RTK Query) + Tailwind v4 + shadcn/ui.

Server state in RTK Query; client state in slices.

## Workspace

Always open `App.code-workspace` so `.cursor/rules/` and `AGENTS.md` apply.
