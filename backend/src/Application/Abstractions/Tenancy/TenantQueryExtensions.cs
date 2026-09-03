using Domain.Identity;
using SharedKernel.Auditing;
using Domain.Abstractions;

namespace Application.Abstractions.Tenancy;

public static class TenantQueryExtensions
{
    public static IQueryable<T> ApplyResourceScope<T>(
        this IQueryable<T> query,
        PermissionScope scope,
        Guid organizationId,
        string? userId)
        where T : class, ITenantScoped, IAuditableEntity
    {
        return scope switch
        {
            PermissionScope.Own => query.Where(x => x.OrganizationId == organizationId && x.CreatedBy == userId),
            PermissionScope.Organization => query.Where(x => x.OrganizationId == organizationId),
            PermissionScope.Subtree or PermissionScope.Global => query,
            _ => query.Where(_ => false)
        };
    }
}
