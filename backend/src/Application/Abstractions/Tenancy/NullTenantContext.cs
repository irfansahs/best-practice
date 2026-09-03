using Domain.Tenancy;

namespace Application.Abstractions.Tenancy;

public sealed class NullTenantContext : ITenantContext
{
    public const string UnavailablePath = "\u0000";

    public static NullTenantContext Instance { get; } = new();

    public bool IsAvailable => false;
    public Guid OrganizationId => Guid.Empty;
    public string OrganizationPath => UnavailablePath;
    public OrganizationType OrganizationType => OrganizationType.Platform;
    public bool IsImpersonating => false;
}
