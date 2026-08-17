using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Localization;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages", Schemas.Localization);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(Language.MaxCodeLength).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(Language.MaxNameLength).IsRequired();
        builder.Property(x => x.NativeName).HasMaxLength(Language.MaxNativeNameLength).IsRequired();
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
