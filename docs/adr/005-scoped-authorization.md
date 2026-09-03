# ADR-005: Scope-Aware Authorization

## Status

Accepted

## Context

RBAC already answered *whether* a user may call an action (`catalog.products.read`). It did not answer *which records*. Cloning permissions per tenant would explode the catalog.

## Decision

- Keep a single `Permission.Code`. Attach `PermissionScope` (Own / Organization / Subtree / Global) on `RolePermission` and optional `PermissionOverride`.
- Resolution: widest role scope wins; Allow override may widen; Deny removes the grant entirely.
- JWT `permission` claims are `{code}:{scopeInt}` (example `catalog.products.read:2`). `PermissionAuthorizationHandler` and `AuthorizationBehavior` compare `scope >= minScope`.
- `IPermissionResolver` is the cache boundary (HybridCache in Infrastructure). Command/query handlers never inject `ICacheService`.
- HTTP `.RequirePermission(code, minScope)` and handler `IAuthorizedRequest` are independent layers. Frontend `Can` / `usePermission` is UX only.

## Consequences

- One permission catalog serves platform, operator, and supplier with different scopes.
- JWT size grows with grant count; if claims exceed ~100, switch to role ids in the token plus resolver (the abstraction already exists).
- `Permission.MaxScope` and `IsPlatformOnly` stop over-granting at write time.
