---
name: DDD Monolith Mimarisi
overview: .NET 10 DDD/CQRS monolith + Vite/React frontend. Canonical rules live in AGENTS.md; this file is the short spec and todo tracker.
todos:
  - id: repo-foundation
    content: Kök iskelet, workspace, AGENTS.md, rules, docs pointer, docker-compose
    status: completed
  - id: backend-solution
    content: App.slnx, Directory.Build/Packages.props, 5 src + 4 test projeleri
    status: completed
  - id: shared-kernel
    content: Entity, AggregateRoot, Result, Error, Guard, auditing primitives
    status: completed
  - id: cqrs-dispatcher
    content: IDispatcher, ICommand/IQuery, Scrutor AddCqrs, delegate cache
    status: completed
  - id: contracts-behaviors
    content: ApiResponse, 6 pipeline behaviors sabit sırada, IAppDbContext, IUnitOfWork, CacheKeys/Tags
    status: completed
  - id: options-config
    content: Jwt, Database, Cache, Log, Lockout IOptions + ValidateOnStart
    status: completed
  - id: domain-model
    content: Identity, Catalog, Localization aggregate'leri + domain events + Errors
    status: completed
  - id: persistence
    content: AppDbContext triple interface, EF configs, interceptors, Repository, migration, seeders
    status: completed
  - id: caching
    content: HybridCache, ICacheService (Infrastructure), ICachedQuery + CachingBehavior, tag invalidation via domain events
    status: completed
  - id: localization
    content: DbTranslationProvider, DbStringLocalizer, Translator, culture provider, aggregate translations
    status: completed
  - id: logging-observability
    content: Serilog → SQL log.Logs + file fallback, audit interceptor, CorrelationId enricher
    status: completed
  - id: auth
    content: ADR-003 bearer access + body refresh, Argon2, JWT, permissions, Auth endpoints
    status: completed
  - id: api-pipeline
    content: IEndpoint, middleware sırası (AGENTS.md), compression, health live/ready, OpenAPI/Scalar, CORS
    status: completed
  - id: catalog-vertical
    content: Products + Categories vertical slices, TypedResults endpoints, ProductMapper
    status: completed
  - id: backend-tests
    content: Domain/Application/Architecture/Integration tests, fakes, DatabaseFixture + rollback
    status: completed
  - id: frontend-scaffold
    content: Vite 8, React 19, Tailwind v4, shadcn, eslint/prettier
    status: completed
  - id: frontend-core
    content: RTK Query, axios refresh mutex + auth bootstrap (ADR-003 body refresh), router, i18n db-backend
    status: completed
  - id: frontend-features
    content: Auth, products, categories, translation manager
    status: completed
  - id: frontend-tests-ci
    content: Vitest, MSW, Playwright e2e, GitHub Actions CI
    status: completed
  - id: docs-teaching
    content: docs/01–07 iskelet pointer + docs/adr
    status: completed
  - id: outbox-backlog
    content: "[BACKLOG] Outbox pattern — dış entegrasyon gerektiğinde"
    status: cancelled
  - id: cached-repo-backlog
    content: "[BACKLOG] CachedProductRepository Scrutor decorator — removed from code"
    status: cancelled
  - id: otel-seq-backlog
    content: "[BACKLOG] OpenTelemetry SDK, Seq"
    status: cancelled
isProject: false
---

# DDD Monolith Spec (2026)

Canonical source: [AGENTS.md](../../AGENTS.md). Do not duplicate middleware order, behavior order, forbidden list, or cache rules here.

## Changelog (spec + code alignment)

| Old | New |
| --- | --- |
| httpOnly refresh cookie | ADR-003: refresh JSON body, localStorage (web) |
| Handler `ICacheService` / `GetOrCreateAsync` | Query cache via `ICachedQuery` only |
| `CachedProductRepository` v1 | Removed; ICachedQuery only |
| `IClock` custom types | BCL `TimeProvider` |
| `AddDbContext` + factory | Factory-only registration |
| Domain event dispatch in SaveChanges | Dispatch after commit |
| Lockout in JwtOptions / User constants | `LockoutOptions` |
| Duplicate Permissions.cs | Application only |
| ChangePassword no endpoint | `POST /auth/change-password` |

## Decisions

- Modular monolith: bounded contexts `Identity`, `Catalog`, `Localization` inside five projects.
- Vertical slice: command/query, handler, validator, DTO in one folder; feature mapper at feature root.
- Custom CQRS dispatcher, no MediatR.
- Result pattern; `Error.Code` = localization key.
- Writes: aggregate + `IRepository<T>` + `IUnitOfWork.SaveChangesAsync`.
- Reads: `IAppDbContext` + `AsNoTracking` projection.
- Cache: HybridCache L1; declarative queries only; invalidation post-commit via domain events.
- Logging: Serilog → SQL + audit table; TraceId from `Activity.Current`.
- Auth: see ADR-003 and AGENTS.md Auth section.

## Layers

| Layer | Project | Notes |
| --- | --- | --- |
| SharedKernel | `SharedKernel` | Zero dependencies |
| Domain | `Domain` | Aggregates, VOs, events, errors |
| Application | `Application` | Handlers, behaviors, abstractions |
| Infrastructure | `Infrastructure` | EF, JWT, cache impl, Serilog |
| Api | `Api` | Endpoints, middleware, DI root |

## v1 Folder Patterns

```
backend/src/
  SharedKernel/     Primitives, Results, Guards, Events, IAggregateRoot
  Domain/           Identity/, Catalog/, Localization/
  Application/      Abstractions/, Behaviors/, {Context}/Features/{Feature}/{Commands|Queries}/...
  Infrastructure/   Persistence/, Security/, Caching/, Localization/, Logging/
  Api/              Endpoints/{Context}/, Extensions/, Middlewares/
```

## Major Versions

| Product | Major |
| --- | --- |
| .NET / EF Core | 10 |
| Vite | 8 |
| React | 19 |
| Tailwind | 4 |
| Redux Toolkit | 2 |
| xunit | v3 |
| Shouldly, Mapperly, HybridCache, Scalar | current stable in CPM |

## CQRS Sketches

```csharp
public interface ICommand : ICommand<Unit>;
public interface ICommand<TResponse> : IRequest<TResponse>;
public interface IQuery<TResponse> : IRequest<TResponse>;
```

```csharp
public sealed class GetProductByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery q, CancellationToken ct) =>
        await db.Products.AsNoTracking()
            .Where(p => p.Id == q.Id)
            .Select(p => new ProductDetailDto(...))
            .FirstOrDefaultAsync(ct) is { } dto
            ? dto
            : CatalogErrors.Product.NotFound;
}
```

## Backlog

Outbox, Redis L2, Aspire, OpenTelemetry SDK, Seq, multi-tenancy, full-text search.
