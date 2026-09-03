using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Roles.Queries.GetRoles;

public sealed class GetRolesQueryHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleListItemDto>>
{
    public async Task<Result<IReadOnlyList<RoleListItemDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var path = tenantContext.IsAvailable ? tenantContext.OrganizationPath : string.Empty;

        var roles = await db.Roles
            .AsNoTracking()
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Where(r => r.IsSystemRole || r.OrganizationId == null || (path != string.Empty && r.OrganizationPath != null && path.StartsWith(r.OrganizationPath)))
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        IReadOnlyList<RoleListItemDto> result = roles.Select(r => new RoleListItemDto(
            r.Id,
            r.Name,
            r.Description,
            r.IsSystemRole,
            r.OrganizationId,
            (int)r.AllowedClients,
            r.RolePermissions.Select(rp => new RolePermissionDto(rp.PermissionId, rp.Permission.Code, (int)rp.Scope)).ToArray())).ToArray();

        return Result<IReadOnlyList<RoleListItemDto>>.Success(result);
    }
}
