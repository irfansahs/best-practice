using Application.Abstractions.Tenancy;
using Domain.Tenancy;

namespace Application.UnitTests.Helpers;

public sealed class FakeTenantContext : ITenantContext
{
    public static readonly Guid DefaultOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111199");
    public static readonly string DefaultPath = "/11111111111111111111111111111199/";

    public static FakeTenantContext Default { get; } = new();

    public bool IsAvailable { get; set; } = true;
    public Guid OrganizationId { get; set; } = DefaultOrganizationId;
    public string OrganizationPath { get; set; } = DefaultPath;
    public OrganizationType OrganizationType { get; set; } = OrganizationType.Operator;
    public bool IsImpersonating { get; set; }
}
