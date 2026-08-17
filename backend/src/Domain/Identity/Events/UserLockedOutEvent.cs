using SharedKernel.Events;

namespace Domain.Identity.Events;

public sealed record UserLockedOutEvent(Guid UserId, string Email, DateTimeOffset LockoutEnd) : DomainEventBase;
