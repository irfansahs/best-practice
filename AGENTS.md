# Architecture

Always open the repository with `App.code-workspace`, not as a single folder. Root folder must be in the workspace for `.cursor/rules/` and this file to apply.

## Stack

| Layer | Project | Responsibility |
| --- | --- | --- |
| SharedKernel | `SharedKernel` | Primitives, Result, guards, no dependencies |
| Domain | `Domain` | Aggregates, value objects, domain events, errors |
| Application | `Application` | CQRS handlers, validators, contracts, behaviors |
| Infrastructure | `Infrastructure` | EF Core, auth, cache, logging, localization |
| Presentation | `Api` | Minimal APIs, middleware, OpenAPI, composition root |
| Tests | `Domain.UnitTests`, `Application.UnitTests`, `Api.IntegrationTests`, `ArchitectureTests` | Unit, integration, architecture enforcement |
| Frontend | `frontend` | Vite + React + Redux Toolkit + Tailwind + shadcn/ui |

Project names have no prefix (`Domain`, not `BestPractice.Domain`). On namespace clash use `global::` or alias; do not hide `Application` with `global using`.

## Dependency Direction

`SharedKernel <- Domain <- Application <- Infrastructure <- Api`

Reverse references fail architecture tests.

## Forbidden List

- MediatR (use custom `IDispatcher`)
- AutoMapper (use Mapperly)
- FluentAssertions (use Shouldly)
- Magic strings for config (use `IOptions<T>`)
- Hardcoded culture or UI strings (use localization keys)
- Exception-driven flow control in handlers (use `Result`)
- `IResult` in endpoints (use `TypedResults`)
- `HybridCache`, `DbContext`, or `ICacheService` in command/query handlers
- `ICommand<Unit>` (use non-generic `ICommand` for void commands)
- Repository properties on `IUnitOfWork`
- Outbox in v1 (in-process domain events only)

## Pipeline Behavior Order

Fixed order — no other behaviors in v1:

1. Logging
2. Authorization (`IAuthorizedRequest` only)
3. Validation
4. Caching (`ICachedQuery` only)
5. Transaction (`ICommand` / `ICommand<T>` only, wrapped in `ExecutionStrategy`)
6. Performance (timing only)

AuthorizationBehavior runs only for `IAuthorizedRequest`. Endpoints use `.RequirePermission(...)` for HTTP boundary rejection; behavior covers handler invocation from other hosts. Do not force every command to implement `IAuthorizedRequest`.

## Middleware Order

Fixed order — do not change:

`ExceptionHandler -> ForwardedHeaders -> Hsts/HttpsRedirection -> SecurityHeaders -> SerilogRequestLogging -> CorrelationId -> ResponseCompression -> RequestLocalization -> Authentication -> Authorization -> RateLimiter -> MapEndpoints`

## New Feature — 7 Steps

1. Domain: entity / value object / domain event / error constants
2. Application: command or query folder + handler + validator + DTO in the same folder
3. Application: add mapping methods to the feature mapper (for example `ProductMapper.cs`)
4. Infrastructure: EF configuration + repository if needed + migration
5. Api: endpoint with `TypedResults`, register in route group
6. Tests: domain unit -> application unit -> integration
7. Docs: ADR in `docs/adr/` when an important decision is made

## Cache

- Query cache: **only** `ICachedQuery` + `CachingBehavior`. Handlers never inject `ICacheService` or call `GetOrCreateAsync`.
- `ICacheService` stays for Infrastructure (translation cache) and test fakes only.
- Keys/tags: `CacheKeys` / `CacheTags`. Culture-aware keys come from `ICultureContext` in the key factory; queries do not build culture into their cache contract.
- Invalidation: domain event handlers, **after commit**. Command/query handlers never call `RemoveByTagAsync`.

Key format: `{bounded-context}:{aggregate}:{discriminator}` — example: `CacheKeys.Product(id)` => `catalog:product:{id}`

## Domain Events

- v1: in-process only, no Outbox.
- `DomainEventInterceptor` collects events; `IDomainEventDispatcher` dispatches **after** `SaveChanges` / transaction commit.
- Cache invalidation, mail, and external side effects do not run inside the same DbTransaction.
- Interceptor ≠ dispatcher; one dispatcher implementation.

## Data Access

- `AppDbContext : DbContext, IAppDbContext, IUnitOfWork` — no separate `UnitOfWork.cs`.
- `IUnitOfWork` = only `SaveChangesAsync`. No repository properties.
- Writes: aggregate + `IRepository<TAggregate>` where needed; command handlers call `IUnitOfWork.SaveChangesAsync`.
- Reads: `IAppDbContext` + `AsNoTracking` projection. Handlers use `IAppDbContext`, not concrete `DbContext`.
- Register **only** `AddDbContextFactory<AppDbContext>`. Scoped request context from factory; background jobs use `IDbContextFactory`.
- `IAggregateRoot` lives in SharedKernel only.
- Strongly-typed IDs map to Guid/string in routes. Each feature DTO lives in its own folder with a single name.

## Auth (ADR-003)

- Access JWT via `Authorization: Bearer` (short-lived, in Redux memory).
- Opaque refresh token in JSON body on `/auth/refresh` and `/auth/logout`; web stores refresh in `localStorage`.
- No refresh cookies. SHA256 hash in DB; rotate on refresh; reuse detection revokes family.
- Single refresh story: **bootstrap** (F5 — no access token → `POST /auth/refresh` with stored refresh + splash) and **axios 401 mutex** (queue in-flight requests, one refresh, replay or logout). See `docs/adr/003-bearer-token-auth.md`.

## v1 API Surface (as-built)

**Auth:** login, register, refresh, logout, me, change-password

**Catalog — Products:** list (paged), get by id, create, update, delete, change price

**Catalog — Categories:** list, get by id, create, update, delete

**Localization:** languages, resources by culture, upsert translation, import translations

**Tenancy:** organizations (tree/CRUD/status), members, roles (scoped grants), permission catalog; `POST /auth/switch-organization`, `GET /auth/organizations`

## Localization Key Convention

Format: `{BoundedContext}.{Entity}.{State}` or `{Layer}.{Rule}`

`Error.Code` equals the localization key. Examples: `Catalog.Product.NotFound`, `Validation.Required`

Frontend i18n: custom `db-backend.ts` only — no `i18next-http-backend`.

## Options

- `JwtOptions`, `DatabaseOptions`, `CacheOptions`, `LogOptions`, `LockoutOptions`
- Lockout settings in `LockoutOptions` — not inside `JwtOptions`

## Permissions

Single format: lowercase dotted strings — `catalog.products.read`. C# constants in `Application/Security/Permissions.cs` only.

## Result Pattern

Implicit operators: `Error` → `Result` / `Result<T>`; `T` → `Result<T>`.

Commands: `ICommand` (void/delete) or `ICommand<TResponse>` (with response). Queries: `IQuery<TResponse>`.

## File Checklist

- [ ] File-scoped namespace
- [ ] Class is `sealed` when applicable
- [ ] Primary constructor used when possible
- [ ] No magic strings (`CacheKeys`, `Error`, localization keys)
- [ ] No thrown exceptions for expected failures (`Result` instead)
- [ ] Handler uses `IAppDbContext`, not concrete `DbContext`
- [ ] Validator and response DTO in the same folder as the command/query
- [ ] Endpoint returns `TypedResults`, not `IResult`

## Testing

- Architecture: dependency direction, Application must not reference SqlServer provider, handler/validator/DTO same folder, sealed, no MediatR/AutoMapper.
- Application unit: no mock framework — `FakeCurrentUser`, `FakeCacheService`, `FakeUnitOfWork`, BCL `FakeTimeProvider` (`Microsoft.Extensions.Time.Testing`).
- Integration: one Testcontainers fixture per class; transaction + rollback per test; `MaxRetryCount = 0` in tests.
- Playwright v1: login → create product → switch language → F5 session persists.

## Commands

```bash
docker compose up -d
docker compose --profile dev up -d --build
docker compose --profile full up -d --build
dotnet build backend/App.slnx
dotnet test backend/App.slnx
dotnet ef migrations add <Name> --project backend/src/Infrastructure --startup-project backend/src/Api
dotnet ef database update --project backend/src/Infrastructure --startup-project backend/src/Api
dotnet ef migrations has-pending-model-changes --project backend/src/Infrastructure --startup-project backend/src/Api
cd frontend && npm install && npm run dev
cd frontend && npm run build && npm run lint && npm run test
```

Automatic `MigrateAsync` / seed run only in **Development**. Outside Development apply migrations with `dotnet ef database update` (or a CI migrate job) and inject `Jwt__SecretKey` / `Database__ConnectionString` via environment — never the repo defaults.

## Backlog (v1 out of scope)

Outbox, Redis L2, Aspire, full-text search, OpenTelemetry SDK, Seq
