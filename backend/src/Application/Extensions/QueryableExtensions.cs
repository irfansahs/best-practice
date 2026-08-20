using Application.Abstractions.Paged;
using Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedList<TResult>> ToPagedListAsync<TSource, TResult>(
        this IQueryable<TSource> query,
        PagedQuery request,
        Func<TSource, TResult> map,
        CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Skip)
            .Take(request.SafePageSize)
            .ToListAsync(cancellationToken);

        var mappedItems = items.Select(map).ToList();

        return PagedList<TResult>.Create(mappedItems, request.SafePage, request.SafePageSize, total);
    }
}
