using SharedKernel.Events;

namespace Domain.Tenancy.Events;

public sealed record OrganizationStatusChangedEvent(
    Guid OrganizationId,
    OrganizationStatus Status) : DomainEventBase;
