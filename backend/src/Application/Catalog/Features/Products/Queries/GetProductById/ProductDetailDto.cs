namespace Application.Catalog.Features.Products.Queries.GetProductById;

public sealed record ProductDetailDto(
    Guid Id,
    string Sku,
    decimal Price,
    string Currency,
    Guid CategoryId,
    Guid LanguageId,
    bool IsActive,
    string Name,
    string? Description,
    string Slug);
