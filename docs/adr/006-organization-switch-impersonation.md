# ADR-006: Organization Switch and Impersonation

## Status

Accepted

## Context

Platform operators (Ranna) must enter an operator or supplier context without a second “god mode” API. Members also switch among their own organizations.

## Decision

- Single endpoint: `POST /auth/switch-organization` `{ organizationId, refreshToken, clientType }`.
- If the user has an active membership → normal switch.
- Else if they hold `tenancy.organizations.impersonate` at **Global** → impersonation (`imp=1` claim, short-lived access token, `RefreshToken.IsImpersonating`).
- New access + refresh tokens are issued; the previous refresh family is revoked.
- Subsequent writes record `AuditLog.IsImpersonated` from `ICurrentUser`.
- `AllowedClients` on roles (Web / Mobile / All) rejects platform admins on mobile without a hardcoded “Ranna” check.

## Consequences

- One client flow for both membership switch and support impersonation.
- Impersonation is visible in JWT, refresh rows, `/auth/me`, and audit — not a hidden header.
- Suspended organizations cannot mint tokens (`Tenancy.Organization.Suspended`).
