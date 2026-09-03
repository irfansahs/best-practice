using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class CatalogSeeder
{
    public static readonly Guid SampleCategoryId = Guid.Parse("55555555-5555-5555-5555-555555555501");
    public static readonly Guid SampleProductId = Guid.Parse("55555555-5555-5555-5555-555555555502");

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var aquacare = await context.Organizations.IgnoreQueryFilters()
            .FirstAsync(o => o.Id == OrganizationSeeder.AquaCareId, cancellationToken);

        var unassignedCategories = await context.Categories.IgnoreQueryFilters()
            .Where(c => c.OrganizationId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var unassignedCategory in unassignedCategories)
            unassignedCategory.AssignTenant(aquacare.Id, aquacare.Path);

        var unassignedProducts = await context.Products.IgnoreQueryFilters()
            .Where(p => p.OrganizationId == Guid.Empty)
            .ToListAsync(cancellationToken);
        foreach (var unassignedProduct in unassignedProducts)
            unassignedProduct.AssignTenant(aquacare.Id, aquacare.Path);

        if (unassignedCategories.Count > 0 || unassignedProducts.Count > 0)
            await context.SaveChangesAsync(cancellationToken);

        if (await context.Products.IgnoreQueryFilters().AnyAsync(cancellationToken)) return;

        var category = Category.Create(SampleCategoryId, aquacare.Id, aquacare.Path).Value;
        category.SetTranslation(LanguageSeeder.EnglishId, "General", "Default category");
        category.SetTranslation(LanguageSeeder.TurkishId, "Genel", "Varsayılan kategori");

        var product = Product.Create(
            SampleProductId,
            Sku.Create("SAMPLE-001").Value,
            Money.Create(19.99m, "USD").Value,
            SampleCategoryId,
            aquacare.Id,
            aquacare.Path).Value;

        product.SetTranslation(LanguageSeeder.EnglishId, "Sample Product", "A sample catalog product.");
        product.SetTranslation(LanguageSeeder.TurkishId, "Örnek Ürün", "Örnek katalog ürünü.");

        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
