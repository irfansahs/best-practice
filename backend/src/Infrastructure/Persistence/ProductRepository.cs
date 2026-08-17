using Application.Catalog.Abstractions;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
{
    public override async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Products
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
