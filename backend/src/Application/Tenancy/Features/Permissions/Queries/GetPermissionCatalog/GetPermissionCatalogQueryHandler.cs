using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Permissions.Queries.GetPermissionCatalog;

public sealed class GetPermissionCatalogQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPermissionCatalogQuery, IReadOnlyList<PermissionCatalogItemDto>>
{
    public async Task<Result<IReadOnlyList<PermissionCatalogItemDto>>> Handle(GetPermissionCatalogQuery request, CancellationToken cancellationToken)
    {
        var items = await db.Permissions.AsNoTracking()
            .OrderBy(p => p.Module).ThenBy(p => p.Code)
            .Select(p => new PermissionCatalogItemDto(p.Id, p.Code, p.Description, p.Module, (int)p.MaxScope, p.IsPlatformOnly))
            .ToListAsync(cancellationToken);

        return items;
    }
}
