using SharedKernel.Events;

namespace Domain.Tenancy.Events;

public sealed record MembershipChangedEvent(Guid UserId, Guid OrganizationId) : DomainEventBase;
