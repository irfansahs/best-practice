using SharedKernel.Events;

namespace Domain.Identity.Events;

public sealed record UserRegisteredEvent(Guid UserId, string Email) : DomainEventBase;
