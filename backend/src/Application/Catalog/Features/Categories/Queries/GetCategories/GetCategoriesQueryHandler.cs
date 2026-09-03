using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.Abstractions.Tenancy;
using Application.Security;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(
    IAppDbContext db,
    ILanguageLookup languages,
    ICurrentUser currentUser) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryListItemDto>>
{
    public async Task<Result<IReadOnlyList<CategoryListItemDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var languageId = await languages.GetCurrentLanguageIdAsync(cancellationToken);

        var query = db.Categories.AsNoTracking().Include(c => c.Translations).AsQueryable();
        if (currentUser.OrganizationId is { } orgId)
        {
            var scope = currentUser.GetScope(Permissions.Catalog.Categories.Read) ?? Domain.Identity.PermissionScope.Organization;
            query = query.ApplyResourceScope(scope, orgId, currentUser.UserId?.ToString());
        }

        var categories = await query
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return categories
            .Select(category => CategoryMapper.ToListItemDto(category, category.Translations.SelectForLanguage(languageId)))
            .ToList();
    }
}
