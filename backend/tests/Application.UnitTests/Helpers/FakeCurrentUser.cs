using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Tenancy;

namespace Application.UnitTests.Helpers;

public sealed class FakeCurrentUser : ICurrentUser
{
    public Guid? UserId { get; set; }

    public string? Email { get; set; }

    public Guid? OrganizationId { get; set; } = FakeTenantContext.DefaultOrganizationId;

    public string? OrganizationPath { get; set; } = FakeTenantContext.DefaultPath;

    public OrganizationType? OrganizationType { get; set; } = Domain.Tenancy.OrganizationType.Operator;

    public ClientType? ClientType { get; set; } = Domain.Identity.ClientType.Web;

    public bool IsImpersonating { get; set; }

    public string? SecurityStamp { get; set; }

    public IReadOnlyDictionary<string, PermissionScope> PermissionMap { get; set; } =
        new Dictionary<string, PermissionScope>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> Permissions => PermissionMap.Keys.ToArray();

    public bool IsAuthenticated => UserId.HasValue;

    public bool HasPermission(string permission, PermissionScope minScope = PermissionScope.Organization) =>
        PermissionMap.TryGetValue(permission, out var scope) && scope >= minScope;

    public PermissionScope? GetScope(string permission) =>
        PermissionMap.TryGetValue(permission, out var scope) ? scope : null;
}
