using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Catalog;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.ProductIdRequired;

        var product = await db.Products.AsNoTracking()
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null) return CatalogErrors.ProductNotFound;

        var languageId = await languages.GetCurrentLanguageIdAsync(cancellationToken);
        var translation = product.Translations.SelectForLanguage(languageId);

        return ProductMapper.ToDetailDto(product, translation);
    }
}
