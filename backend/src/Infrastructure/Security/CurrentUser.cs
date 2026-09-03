using Application.Abstractions.Security;
using Application.Abstractions.Tenancy;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Security;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId => Guid.TryParse(
        User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue(ClaimTypes.Name),
        out var id) ? id : null;

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public Guid? OrganizationId => Guid.TryParse(User?.FindFirstValue(AuthClaims.OrganizationId), out var id) ? id : null;

    public string? OrganizationPath => User?.FindFirstValue(AuthClaims.OrganizationPath);

    public OrganizationType? OrganizationType =>
        Enum.TryParse<OrganizationType>(User?.FindFirstValue(AuthClaims.OrganizationType), true, out var type) ? type : null;

    public ClientType? ClientType =>
        Enum.TryParse<ClientType>(User?.FindFirstValue(AuthClaims.ClientType), true, out var type) ? type : null;

    public bool IsImpersonating => User?.FindFirstValue(AuthClaims.Impersonating) == "1";

    public string? SecurityStamp => User?.FindFirstValue(AuthClaims.SecurityStamp);

    public IReadOnlyDictionary<string, PermissionScope> PermissionMap
    {
        get
        {
            var map = new Dictionary<string, PermissionScope>(StringComparer.OrdinalIgnoreCase);
            if (User is null) return map;

            foreach (var claim in User.FindAll(AuthClaims.Permission))
            {
                if (!PermissionClaimFormatter.TryParse(claim.Value, out var code, out var scope)) continue;
                if (!map.TryGetValue(code, out var existing) || scope > existing)
                    map[code] = scope;
            }

            return map;
        }
    }

    public IReadOnlyCollection<string> Permissions => PermissionMap.Keys.ToArray();

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool HasPermission(string permission, PermissionScope minScope = PermissionScope.Organization) =>
        PermissionMap.TryGetValue(permission, out var scope) && scope >= minScope;

    public PermissionScope? GetScope(string permission) =>
        PermissionMap.TryGetValue(permission, out var scope) ? scope : null;
}
