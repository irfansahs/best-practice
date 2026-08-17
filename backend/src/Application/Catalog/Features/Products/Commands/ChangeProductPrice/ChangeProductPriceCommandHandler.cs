using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Commands.ChangeProductPrice;

public sealed class ChangeProductPriceCommandHandler(IAppDbContext db) : IRequestHandler<ChangeProductPriceCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ChangeProductPriceCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.ProductIdRequired;

        var priceResult = Money.Create(request.Price, request.Currency);
        if (priceResult.IsFailure) return priceResult.Error;

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null) return CatalogErrors.ProductNotFound;

        var changeResult = product.ChangePrice(priceResult.Value);
        if (changeResult.IsFailure) return changeResult.Error;

        return Unit.Value;
    }
}
