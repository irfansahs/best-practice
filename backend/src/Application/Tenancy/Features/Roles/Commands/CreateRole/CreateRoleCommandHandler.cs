using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Tenancy;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    public async Task<Result<CreateRoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsAvailable) return TenancyErrors.TenantContextRequired;

        var created = Role.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            tenantContext.OrganizationId,
            tenantContext.OrganizationPath,
            isSystemRole: false,
            allowedClients: (ClientTypes)request.AllowedClients);

        if (created.IsFailure) return created.Error;

        var permissionIds = request.Grants.Select(p => p.PermissionId).Distinct().ToArray();
        var permissions = await db.Permissions.Where(p => permissionIds.Contains(p.Id)).ToListAsync(cancellationToken);
        if (permissions.Count != permissionIds.Length) return IdentityErrors.PermissionNotFound;

        foreach (var input in request.Grants)
        {
            var permission = permissions.First(p => p.Id == input.PermissionId);
            var grant = created.Value.GrantPermission(permission, (PermissionScope)input.Scope);
            if (grant.IsFailure) return grant.Error;
        }

        db.Roles.Add(created.Value);
        return new CreateRoleResponse(created.Value.Id);
    }
}
