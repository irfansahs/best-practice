using SharedKernel.Events;

namespace Domain.Tenancy.Events;

public sealed record OrganizationCreatedEvent(
    Guid OrganizationId,
    Guid? ParentId,
    OrganizationType Type,
    string Path) : DomainEventBase;
