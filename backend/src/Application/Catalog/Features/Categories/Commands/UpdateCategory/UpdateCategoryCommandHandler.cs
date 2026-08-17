using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.CategoryIdRequired;

        if (request.ParentCategoryId.HasValue && request.ParentCategoryId != Guid.Empty)
        {
            var parentExists = await db.Categories.AsNoTracking()
                .AnyAsync(c => c.Id == request.ParentCategoryId, cancellationToken);
            if (!parentExists) return CatalogErrors.CategoryNotFound;
        }

        if (!await languages.ExistsAsync(request.LanguageId, cancellationToken))
            return CatalogErrors.TranslationLanguageRequired;

        var category = await db.Categories
            .Include(c => c.Translations)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (category is null) return CatalogErrors.CategoryNotFound;

        var parentResult = category.AssignParent(request.ParentCategoryId);
        if (parentResult.IsFailure) return parentResult.Error;

        var translationResult = category.SetTranslation(request.LanguageId, request.Name, request.Description);
        if (translationResult.IsFailure) return translationResult.Error;

        var statusResult = request.IsActive ? category.Activate() : category.Deactivate();
        if (statusResult.IsFailure) return statusResult.Error;

        return Unit.Value;
    }
}
