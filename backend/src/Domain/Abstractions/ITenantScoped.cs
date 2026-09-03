namespace Domain.Abstractions;

public interface ITenantScoped
{
    Guid OrganizationId { get; }

    string OrganizationPath { get; }

    void AssignTenant(Guid organizationId, string organizationPath);
}
