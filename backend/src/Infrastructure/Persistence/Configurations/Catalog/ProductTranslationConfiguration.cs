using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

internal sealed class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTranslation> builder)
    {
        builder.ToTable("ProductTranslations", Schemas.Catalog);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(Product.MaxTranslationNameLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(Product.MaxTranslationDescriptionLength);
        builder.Property(x => x.Slug).HasConversion(s => s.Value, v => Slug.Create(v).Value).HasMaxLength(Slug.MaxLength).IsRequired();
        builder.HasIndex(x => new { x.ProductId, x.LanguageId }).IsUnique();
    }
}
