using SharedKernel.Events;

namespace Domain.Identity.Events;

public sealed record RolePermissionsChangedEvent(Guid RoleId) : DomainEventBase;
