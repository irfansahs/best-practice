using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", Schemas.Identity);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(Role.MaxNameLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(Role.MaxDescriptionLength);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasMany(x => x.Permissions).WithMany().UsingEntity(j => j.ToTable("RolePermissions", Schemas.Identity));
        builder.Navigation(x => x.Permissions).HasField("_permissions");
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
