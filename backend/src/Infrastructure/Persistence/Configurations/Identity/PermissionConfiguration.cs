using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", Schemas.Identity);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(Permission.MaxCodeLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(Permission.MaxDescriptionLength);
        builder.Property(x => x.Module).HasMaxLength(Permission.MaxModuleLength).IsRequired();
        builder.Property(x => x.MaxScope).HasConversion<int>().IsRequired();
        builder.Property(x => x.IsPlatformOnly).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.Module);
    }
}
