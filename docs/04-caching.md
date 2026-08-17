# Caching

## Service

Handlers use `ICacheService.GetOrCreateAsync` — never inject `HybridCache` directly.

## Keys

Format: `{bounded-context}:{aggregate}:{discriminator}`

Defined in `Application/Caching/CacheKeys.cs`.

## Declarative Query Cache

Queries implementing `ICachedQuery` are cached by `CachingBehavior`.

## Invalidation

Domain event handlers call `RemoveByTagAsync` using tags from `CacheTags`.

Command/query handlers must not invalidate cache directly.

## Repository Decorator

`CachedProductRepository` wraps `IProductRepository` via Scrutor `Decorate`.

## Future L2

Add Redis to HybridCache registration without changing application code.
