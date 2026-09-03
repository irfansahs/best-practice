using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Roles.Commands.UpdateRolePermissions;

public sealed class UpdateRolePermissionsCommandHandler(IAppDbContext db) : IRequestHandler<UpdateRolePermissionsCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await db.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);
        if (role is null) return IdentityErrors.RoleNotFound;
        if (role.IsSystemRole) return IdentityErrors.SystemRoleProtected;

        var permissionIds = request.Grants.Select(p => p.PermissionId).Distinct().ToArray();
        var permissions = await db.Permissions.Where(p => permissionIds.Contains(p.Id)).ToListAsync(cancellationToken);
        if (permissions.Count != permissionIds.Length) return IdentityErrors.PermissionNotFound;

        foreach (var existing in role.RolePermissions.ToArray())
            role.RevokePermission(existing.PermissionId);

        foreach (var input in request.Grants)
        {
            var permission = permissions.First(p => p.Id == input.PermissionId);
            var grant = role.GrantPermission(permission, (PermissionScope)input.Scope);
            if (grant.IsFailure) return grant.Error;
        }

        return Unit.Value;
    }
}
