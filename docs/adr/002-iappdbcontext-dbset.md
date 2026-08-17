# ADR-002: IAppDbContext Exposes DbSet Properties

## Status

Accepted

## Context

Strict repository-per-query adds boilerplate without benefit when EF Core already implements repository + unit of work.

## Decision

`Application` references EF Core core package only. Handlers use `IAppDbContext` with `DbSet<T>` for reads.

Writes still go through aggregate repositories where appropriate.

## Consequences

- Pragmatic LINQ projections in query handlers
- Architecture test blocks Application -> Infrastructure and Application -> SqlServer provider references
