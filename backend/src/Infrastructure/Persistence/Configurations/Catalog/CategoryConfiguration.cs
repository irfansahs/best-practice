using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Catalog;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", Schemas.Catalog);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.OrganizationPath).HasMaxLength(450).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property<byte[]>("RowVersion").IsRowVersion();
        builder.HasMany(x => x.Translations).WithOne().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Translations).HasField("_translations");
        builder.HasIndex(x => x.ParentCategoryId);
        builder.HasIndex(x => x.OrganizationPath);
    }
}
