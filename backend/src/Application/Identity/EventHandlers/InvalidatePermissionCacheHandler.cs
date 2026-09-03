using Application.Abstractions.Caching;
using Application.Abstractions.Events;
using Application.Caching;
using Domain.Identity.Events;
using Domain.Tenancy.Events;

namespace Application.Identity.EventHandlers;

public sealed class InvalidatePermissionCacheOnMembershipChangedHandler(ICacheService cache)
    : IDomainEventHandler<MembershipChangedEvent>
{
    public Task Handle(MembershipChangedEvent domainEvent, CancellationToken cancellationToken) =>
        cache.RemoveByTagAsync(CacheTags.Permissions, cancellationToken).AsTask();
}

public sealed class InvalidatePermissionCacheOnRoleChangedHandler(ICacheService cache)
    : IDomainEventHandler<RolePermissionsChangedEvent>
{
    public Task Handle(RolePermissionsChangedEvent domainEvent, CancellationToken cancellationToken) =>
        cache.RemoveByTagAsync(CacheTags.Permissions, cancellationToken).AsTask();
}
