using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

internal sealed class CategoryTranslationConfiguration : IEntityTypeConfiguration<CategoryTranslation>
{
    public void Configure(EntityTypeBuilder<CategoryTranslation> builder)
    {
        builder.ToTable("CategoryTranslations", Schemas.Catalog);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(Category.MaxTranslationNameLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(Category.MaxTranslationDescriptionLength);
        builder.Property(x => x.Slug).HasConversion(s => s.Value, v => Slug.Create(v).Value).HasMaxLength(Slug.MaxLength).IsRequired();
        builder.HasIndex(x => new { x.CategoryId, x.LanguageId }).IsUnique();
    }
}
