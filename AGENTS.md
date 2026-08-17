# Architecture

Always open the repository with `App.code-workspace`, not as a single folder. Root folder must be in the workspace for `.cursor/rules/` and this file to apply.

## Stack (single view)

| Layer | Project | Responsibility |
| --- | --- | --- |
| SharedKernel | `SharedKernel` | Primitives, Result, guards, no dependencies |
| Domain | `Domain` | Aggregates, value objects, domain events, errors |
| Application | `Application` | CQRS handlers, validators, contracts, behaviors |
| Infrastructure | `Infrastructure` | EF Core, auth, cache, logging, localization |
| Presentation | `Api` | Minimal APIs, middleware, OpenAPI, composition root |
| Tests | `Domain.UnitTests`, `Application.UnitTests`, `Api.IntegrationTests`, `ArchitectureTests` | Unit, integration, architecture enforcement |
| Frontend | `frontend` | Vite + React + Redux Toolkit + Tailwind + shadcn/ui |

## Dependency Direction

`SharedKernel <- Domain <- Application <- Infrastructure <- Api`

Arrows point from dependent to dependency. Reverse references fail architecture tests.

## Forbidden List

- MediatR (use custom `IDispatcher`)
- AutoMapper (use Mapperly)
- FluentAssertions (use Shouldly)
- Magic strings for config (use `IOptions<T>`)
- Hardcoded culture or UI strings (use localization keys)
- Exception-driven flow control in handlers (use `Result`)
- `IResult` in endpoints (use `TypedResults`)
- Direct `HybridCache` or `DbContext` in handlers (use `ICacheService` / `IAppDbContext`)
- Repository properties on `IUnitOfWork`
- Outbox in v1 (in-process domain events only)

## New Feature — 7 Steps

1. Domain: entity / value object / domain event / error constants
2. Application: command or query folder + handler + validator + DTO in the same folder
3. Application: add mapping methods to the feature mapper (for example `ProductMapper.cs`)
4. Infrastructure: EF configuration + repository if needed + migration
5. Api: endpoint with `TypedResults`, register in route group
6. Tests: domain unit -> application unit -> integration
7. Docs: ADR when an important decision is made

## Cache Key Convention

Format: `{bounded-context}:{aggregate}:{discriminator}`

Example: `CacheKeys.Product(id)` => `catalog:product:{id}`

## Localization Key Convention

Format: `{BoundedContext}.{Entity}.{State}` or `{Layer}.{Rule}`

Examples: `Catalog.Product.NotFound`, `Validation.Required`

## File Checklist (every generated file)

- [ ] File-scoped namespace
- [ ] Class is `sealed` when applicable
- [ ] Primary constructor used when possible
- [ ] No magic strings (`CacheKeys`, `Error`, localization keys)
- [ ] No thrown exceptions for expected failures (`Result` instead)
- [ ] Handler uses `IAppDbContext`, not concrete `DbContext`
- [ ] Validator lives in the same folder as the command/query
- [ ] Response DTO lives in the same folder as the command/query
- [ ] Endpoint returns `TypedResults`, not `IResult`

## Pipeline Behavior Order

`Logging -> Authorization -> Validation -> Caching -> Transaction -> Performance`

## Middleware Order

`ExceptionHandler -> Hsts/HttpsRedirection -> SecurityHeaders -> SerilogRequestLogging -> CorrelationId -> ResponseCompression -> RequestLocalization -> Authentication -> Authorization -> RateLimiter -> MapEndpoints`

## Commands

```bash
docker compose up -d
dotnet build backend/App.slnx
dotnet test backend/App.slnx
dotnet ef migrations add <Name> --project backend/src/Infrastructure --startup-project backend/src/Api
dotnet ef database update --project backend/src/Infrastructure --startup-project backend/src/Api
dotnet ef migrations has-pending-model-changes --project backend/src/Infrastructure --startup-project backend/src/Api
cd frontend && npm install && npm run dev
cd frontend && npm run build && npm run lint && npm run test
```