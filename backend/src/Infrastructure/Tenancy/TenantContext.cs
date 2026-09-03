using System.Security.Claims;
using Application.Abstractions.Security;
using Application.Abstractions.Tenancy;
using Domain.Tenancy;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Tenancy;

public sealed class TenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private static readonly AsyncLocal<ITenantContext?> Override = new();

    public static IDisposable Use(ITenantContext context)
    {
        var previous = Override.Value;
        Override.Value = context;
        return new Restore(previous);
    }

    public static IDisposable UseSystem() => Use(SystemTenantContext.Instance);

    public bool IsAvailable => Current.IsAvailable;
    public Guid OrganizationId => Current.OrganizationId;
    public string OrganizationPath => Current.OrganizationPath;
    public OrganizationType OrganizationType => Current.OrganizationType;
    public bool IsImpersonating => Current.IsImpersonating;

    private ITenantContext Current
    {
        get
        {
            if (Override.Value is not null) return Override.Value;

            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return NullTenantContext.Instance;

            var orgIdValue = user.FindFirstValue(AuthClaims.OrganizationId);
            var path = user.FindFirstValue(AuthClaims.OrganizationPath);
            if (!Guid.TryParse(orgIdValue, out var orgId) || string.IsNullOrWhiteSpace(path))
                return NullTenantContext.Instance;

            Enum.TryParse<OrganizationType>(user.FindFirstValue(AuthClaims.OrganizationType), ignoreCase: true, out var type);
            var impersonating = user.FindFirstValue(AuthClaims.Impersonating) == "1";
            return new Snapshot(orgId, path, type, impersonating);
        }
    }

    private sealed class Restore(ITenantContext? previous) : IDisposable
    {
        public void Dispose() => Override.Value = previous;
    }

    private sealed record Snapshot(Guid OrganizationId, string OrganizationPath, OrganizationType OrganizationType, bool IsImpersonating) : ITenantContext
    {
        public bool IsAvailable => true;
    }
}

public sealed class SystemTenantContext : ITenantContext
{
    public static SystemTenantContext Instance { get; } = new();

    public bool IsAvailable => true;
    public Guid OrganizationId => Guid.Empty;
    public string OrganizationPath => "/";
    public OrganizationType OrganizationType => OrganizationType.Platform;
    public bool IsImpersonating => false;
}
