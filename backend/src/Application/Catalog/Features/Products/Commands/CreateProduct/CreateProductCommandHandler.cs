using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Application.Abstractions.Tenancy;
using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(IAppDbContext db, ILanguageLookup languages, ITenantContext tenantContext) : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsAvailable) return TenancyErrors.TenantContextRequired;
        var skuResult = Sku.Create(request.Sku);
        if (skuResult.IsFailure) return skuResult.Error;

        var priceResult = Money.Create(request.Price, request.Currency);
        if (priceResult.IsFailure) return priceResult.Error;

        if (request.CategoryId == Guid.Empty) return CatalogErrors.CategoryIdRequired;

        var categoryExists = await db.Categories.AsNoTracking().AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists) return CatalogErrors.CategoryNotFound;

        if (!await languages.ExistsAsync(request.LanguageId, cancellationToken))
            return CatalogErrors.TranslationLanguageRequired;

        var skuExists = await db.Products.AsNoTracking().AnyAsync(p => p.Sku == skuResult.Value, cancellationToken);
        if (skuExists) return CatalogErrors.SkuAlreadyExists;

        var productResult = Product.Create(
            Guid.NewGuid(),
            skuResult.Value,
            priceResult.Value,
            request.CategoryId,
            tenantContext.OrganizationId,
            tenantContext.OrganizationPath);
        if (productResult.IsFailure) return productResult.Error;

        var product = productResult.Value;
        var translationResult = product.SetTranslation(request.LanguageId, request.Name, request.Description);
        if (translationResult.IsFailure) return translationResult.Error;

        db.Products.Add(product);
        return new CreateProductResponse(product.Id, product.Sku.Value);
    }
}
