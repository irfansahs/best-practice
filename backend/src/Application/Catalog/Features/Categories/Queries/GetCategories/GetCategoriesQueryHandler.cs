using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryListItemDto>>
{
    public async Task<Result<IReadOnlyList<CategoryListItemDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var languageId = await languages.GetCurrentLanguageIdAsync(cancellationToken);

        var categories = await db.Categories.AsNoTracking()
            .Include(c => c.Translations)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return categories
            .Select(category => CategoryMapper.ToListItemDto(category, category.Translations.SelectForLanguage(languageId)))
            .ToList();
    }
}
