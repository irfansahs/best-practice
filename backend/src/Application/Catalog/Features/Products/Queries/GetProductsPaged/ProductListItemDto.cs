namespace Application.Catalog.Features.Products.Queries.GetProductsPaged;

public sealed record ProductListItemDto(
    Guid Id,
    string Sku,
    decimal Price,
    string Currency,
    bool IsActive,
    string Name);
