# Localization

## Two Layers

1. **System/UI strings** — `localization.Languages`, `localization.TranslationEntries`
2. **Business translations** — aggregate child collections (for example `ProductTranslation`)

## Key Format

`BoundedContext.Entity.State` or `Layer.Rule`

Examples: `Catalog.Product.NotFound`, `Validation.Required`

## API

- `GET /api/v1/localization/languages`
- `GET /api/v1/localization/resources/{culture}` (ETag supported)
- `PUT /api/v1/localization/translations`

## Frontend

i18next loads resources from the API via `shared/i18n/db-backend.ts`.

Culture resolution order on API:

`?culture=` -> `X-Culture` header -> JWT claim -> `Accept-Language` -> DB default language

## Cache

Translation bundles cached with tag `i18n`; invalidate on upsert/import.
