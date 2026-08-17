using Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Localization;

internal sealed class TranslationEntryConfiguration : IEntityTypeConfiguration<TranslationEntry>
{
    public void Configure(EntityTypeBuilder<TranslationEntry> builder)
    {
        builder.ToTable("TranslationEntries", Schemas.Localization);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Namespace).HasMaxLength(TranslationEntry.MaxNamespaceLength).IsRequired();
        builder.Property(x => x.Key).HasMaxLength(TranslationEntry.MaxKeyLength).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(TranslationEntry.MaxValueLength).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => new { x.LanguageId, x.Namespace, x.Key }).IsUnique();
    }
}
