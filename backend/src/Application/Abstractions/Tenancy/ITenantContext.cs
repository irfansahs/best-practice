using Domain.Identity;
using Domain.Tenancy;

namespace Application.Abstractions.Tenancy;

public interface ITenantContext
{
    bool IsAvailable { get; }
    Guid OrganizationId { get; }
    string OrganizationPath { get; }
    OrganizationType OrganizationType { get; }
    bool IsImpersonating { get; }
}
