using Domain.Catalog;
using Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", Schemas.Catalog);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sku).HasConversion(s => s.Value, v => Sku.Create(v).Value).HasMaxLength(Sku.MaxLength).IsRequired();
        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount).HasColumnName("PriceAmount").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(Money.MaxCurrencyLength).IsRequired();
        });
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property<byte[]>("RowVersion").IsRowVersion();
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Translations).WithOne().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Translations).HasField("_translations");
        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasIndex(x => x.CategoryId);
    }
}
