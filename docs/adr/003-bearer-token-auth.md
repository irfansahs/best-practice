# ADR-003: Bearer Access + Body Refresh Tokens (No Cookies)

## Status

Accepted

## Context

SPA and future mobile clients need one auth contract. Cookie-based refresh tied the API to browser SameSite/CORS/`withCredentials` and blocked native mobile SecureStore flows.

## Decision

- Short-lived access JWT via `Authorization: Bearer`
- Opaque refresh token in JSON body on `/auth/refresh` and `/auth/logout`
- No refresh cookies
- Store refresh as SHA256 hash in DB; O(1) lookup by `TokenHash` (not Argon2 scan)
- Rotate refresh on every successful refresh with sliding expiry (`now + RefreshTokenDays`)
- Web: access in memory, refresh in `localStorage`; mobile later uses SecureStore with the same API

## Consequences

- Single contract for web and mobile
- XSS can steal refresh from `localStorage` (accept for SPA; mitigate with short access TTL + rotation/reuse detection)
- Clients must send refresh in body; cookie proxying is unnecessary
