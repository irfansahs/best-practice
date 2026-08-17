using Application.Abstractions.Data;
using Application.Abstractions.Localization;
using Application.Abstractions.Messaging;
using Application.Catalog.Features.Products;
using Application.Contracts;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Queries.GetProductsPaged;

public sealed class GetProductsPagedQueryHandler(IAppDbContext db, ILanguageLookup languages) : IRequestHandler<GetProductsPagedQuery, PagedList<ProductListItemDto>>
{
    public async Task<Result<PagedList<ProductListItemDto>>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page.Page < 1 ? 1 : request.Page.Page;
        var pageSize = request.Page.PageSize is < 1 or > 100 ? 20 : request.Page.PageSize;
        var query = db.Products.AsNoTracking().Include(p => p.Translations).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToUpperInvariant();
            query = query.Where(p => EF.Property<string>(p, "Sku").Contains(term));
        }

        var languageId = await languages.GetCurrentLanguageIdAsync(cancellationToken);

        var total = await query.CountAsync(cancellationToken);
        var products = await query
            .OrderBy(p => EF.Property<string>(p, "Sku"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = products
            .Select(p => ProductMapper.ToListItemDto(p, p.Translations.SelectForLanguage(languageId)))
            .ToList();

        return PagedList<ProductListItemDto>.Create(items, page, pageSize, total);
    }
}
