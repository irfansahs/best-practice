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
        if (await context.Products.AnyAsync(cancellationToken)) return;

        var category = Category.Create(SampleCategoryId).Value;
        category.SetTranslation(LanguageSeeder.EnglishId, "General", "Default category");
        category.SetTranslation(LanguageSeeder.TurkishId, "Genel", "Varsayılan kategori");

        var product = Product.Create(
            SampleProductId,
            Sku.Create("SAMPLE-001").Value,
            Money.Create(19.99m, "USD").Value,
            SampleCategoryId).Value;

        product.SetTranslation(LanguageSeeder.EnglishId, "Sample Product", "A sample catalog product.");
        product.SetTranslation(LanguageSeeder.TurkishId, "Örnek Ürün", "Örnek katalog ürünü.");

        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
