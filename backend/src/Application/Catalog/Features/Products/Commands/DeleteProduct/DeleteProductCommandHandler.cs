using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(IAppDbContext db, TimeProvider timeProvider, ICurrentUser currentUser) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.ProductIdRequired;

        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null) return CatalogErrors.ProductNotFound;

        product.SoftDelete(timeProvider.GetUtcNow(), currentUser.UserId?.ToString());
        return Unit.Value;
    }
}
