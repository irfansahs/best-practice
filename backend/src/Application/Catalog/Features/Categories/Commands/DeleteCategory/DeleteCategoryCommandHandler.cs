using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Catalog.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(IAppDbContext db) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty) return CatalogErrors.CategoryIdRequired;

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (category is null) return CatalogErrors.CategoryNotFound;

        var hasProducts = await db.Products.AsNoTracking()
            .AnyAsync(p => p.CategoryId == request.Id, cancellationToken);
        if (hasProducts) return CatalogErrors.CategoryHasProducts;

        category.SoftDelete(DateTimeOffset.UtcNow);
        return Unit.Value;
    }
}
