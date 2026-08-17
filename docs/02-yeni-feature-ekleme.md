# Adding a New Feature (7 Steps)

1. **Domain** — entity/value object, domain events, `*Errors.cs` constants
2. **Application** — create `Commands/<Name>/` or `Queries/<Name>/` with command/query, handler, validator, DTO in the same folder
3. **Application** — extend feature mapper (for example `ProductMapper.cs`)
4. **Infrastructure** — EF configuration, repository if needed, migration
5. **Api** — endpoint with `TypedResults`, `.Produces<T>()`, route group registration
6. **Tests** — domain unit -> application unit -> integration
7. **Docs** — ADR if the decision is non-obvious

## Example: Catalog Product Command

```
Application/Catalog/Features/Products/Commands/CreateProduct/
  CreateProductCommand.cs
  CreateProductCommandHandler.cs
  CreateProductCommandValidator.cs
  CreateProductResponse.cs
```

## Frontend Slice

```
frontend/src/features/<feature>/
  <feature>.api.ts      # injectEndpoints
  <feature>.slice.ts    # UI-only state
  pages/
  components/
```

Use i18n keys for all user-visible text.
