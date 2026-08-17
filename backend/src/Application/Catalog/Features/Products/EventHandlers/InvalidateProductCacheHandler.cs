using Application.Abstractions.Caching;
using Application.Abstractions.Events;
using Application.Caching;
using Domain.Catalog.Events;

namespace Application.Catalog.Features.Products.EventHandlers;

public sealed class InvalidateProductCacheHandler(ICacheService cache) :
    IDomainEventHandler<ProductCreatedEvent>,
    IDomainEventHandler<ProductPriceChangedEvent>
{
    public Task Handle(ProductCreatedEvent domainEvent, CancellationToken cancellationToken) =>
        cache.RemoveByTagAsync(CacheTags.Products, cancellationToken).AsTask();

    public Task Handle(ProductPriceChangedEvent domainEvent, CancellationToken cancellationToken) =>
        cache.RemoveByTagAsync(CacheTags.Products, cancellationToken).AsTask();
}
