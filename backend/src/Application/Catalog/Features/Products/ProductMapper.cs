using Application.Catalog.Features.Products.Queries.GetProductById;
using Application.Catalog.Features.Products.Queries.GetProductsPaged;
using Domain.Catalog;
using Riok.Mapperly.Abstractions;

namespace Application.Catalog.Features.Products;

[Mapper]
public static partial class ProductMapper
{
    public static ProductDetailDto ToDetailDto(Product product, ProductTranslation? translation) => new(
        product.Id,
        product.Sku.Value,
        product.Price.Amount,
        product.Price.Currency,
        product.CategoryId,
        translation?.LanguageId ?? Guid.Empty,
        product.IsActive,
        translation?.Name ?? string.Empty,
        translation?.Description,
        translation?.Slug.Value ?? string.Empty);

    public static ProductListItemDto ToListItemDto(Product product, ProductTranslation? translation) => new(
        product.Id,
        product.Sku.Value,
        product.Price.Amount,
        product.Price.Currency,
        product.IsActive,
        translation?.Name ?? string.Empty);
}
