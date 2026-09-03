using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.Abstractions.Tenancy;
using Application.Contracts;
using Application.Extensions;
using Application.Security;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Queries.GetProductsPaged;

public sealed class GetProductsPagedQueryHandler(
    IAppDbContext db,
    ILanguageLookup languages,
    ICurrentUser currentUser) : IRequestHandler<GetProductsPagedQuery, PagedList<ProductListItemDto>> {
    public async Task<Result<PagedList<ProductListItemDto>>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken) {

        var query = db.Products.AsNoTracking().Include(p => p.Translations).AsQueryable();
        if (currentUser.OrganizationId is { } orgId)
        {
            var scope = currentUser.GetScope(Permissions.Catalog.Products.Read) ?? Domain.Identity.PermissionScope.Organization;
            query = query.ApplyResourceScope(scope, orgId, currentUser.UserId?.ToString());
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p => EF.Property<string>(p, "Sku").Contains(term));
        }

        var languageId = await languages.GetCurrentLanguageIdAsync(cancellationToken);

        return await query
            .OrderBy(p => EF.Property<string>(p, "Sku"))
            .ToPagedListAsync(
                request,
                p => ProductMapper.ToListItemDto(p, p.Translations.SelectForLanguage(languageId)),
                cancellationToken);
    }
}