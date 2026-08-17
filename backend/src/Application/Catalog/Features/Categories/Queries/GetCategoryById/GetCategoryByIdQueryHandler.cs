using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Catalog;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<GetCategoryByIdQuery, CategoryDetailDto>
{
    public async Task<Result<CategoryDetailDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.CategoryIdRequired;

        var category = await db.Categories.AsNoTracking()
            .Include(c => c.Translations)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null) return CatalogErrors.CategoryNotFound;

        var languageId = await languages.GetCurrentLanguageIdAsync(cancellationToken);
        var translation = category.Translations.SelectForLanguage(languageId);

        return CategoryMapper.ToDetailDto(category, translation);
    }
}
