# ADR-004: Multi-Tenancy via Organization Hierarchy

## Status

Accepted

## Context

Catalog and future AquaCare modules need tenant isolation without a separate database per customer. The business hierarchy is Ranna (platform) → operator → supplier.

## Decision

- Bounded context name: **Tenancy**. Table name: **Organization** (not Company or Tenant).
- Schema: `tenancy`. Membership is a first-class `Membership` aggregate, not a user-organization join with roles collapsed into one row.
- Isolation uses a materialized `Path` (`/{guid:N}/.../`) plus denormalized `OrganizationPath` on `ITenantScoped` resources.
- Global query filter: `OrganizationPath.StartsWith(currentOrgPath)`. Ranna's root path is the whole tree, so platform access needs no special-case branch.
- Writes are stamped by `TenantScopeInterceptor`. Background work uses `SystemTenantContext` (path `/`) explicitly; a missing tenant context does not open all rows.

## Consequences

- Sibling organizations cannot see each other; subtree reads are a single prefix seek.
- Login, refresh, and switch queries must `IgnoreQueryFilters()` because there is no JWT tenant yet.
- v1 stays single-database. Outbox, Redis L2, and true multi-tenant *databases* remain out of scope.
