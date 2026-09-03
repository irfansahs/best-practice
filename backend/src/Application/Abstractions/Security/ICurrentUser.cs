using Domain.Identity;
using Domain.Tenancy;

namespace Application.Abstractions.Security;

public static class AuthClaims
{
    public const string Permission = "permission";
    public const string OrganizationId = "org_id";
    public const string OrganizationPath = "org_path";
    public const string OrganizationType = "org_type";
    public const string ClientType = "client";
    public const string Impersonating = "imp";
    public const string SecurityStamp = "sstamp";
}

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    Guid? OrganizationId { get; }
    string? OrganizationPath { get; }
    OrganizationType? OrganizationType { get; }
    ClientType? ClientType { get; }
    bool IsImpersonating { get; }
    string? SecurityStamp { get; }
    IReadOnlyDictionary<string, PermissionScope> PermissionMap { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string permission, PermissionScope minScope = PermissionScope.Organization);
    PermissionScope? GetScope(string permission);
}
