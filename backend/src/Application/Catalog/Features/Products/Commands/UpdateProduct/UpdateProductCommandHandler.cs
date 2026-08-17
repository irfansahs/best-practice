using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.ProductIdRequired;
        if (request.CategoryId == Guid.Empty) return CatalogErrors.CategoryIdRequired;

        var categoryExists = await db.Categories.AsNoTracking().AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists) return CatalogErrors.CategoryNotFound;

        if (!await languages.ExistsAsync(request.LanguageId, cancellationToken))
            return CatalogErrors.TranslationLanguageRequired;

        var product = await db.Products
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null) return CatalogErrors.ProductNotFound;

        var categoryResult = product.AssignCategory(request.CategoryId);
        if (categoryResult.IsFailure) return categoryResult.Error;

        var translationResult = product.SetTranslation(request.LanguageId, request.Name, request.Description);
        if (translationResult.IsFailure) return translationResult.Error;

        var statusResult = request.IsActive ? product.Activate() : product.Deactivate();
        if (statusResult.IsFailure) return statusResult.Error;

        return Unit.Value;
    }
}
