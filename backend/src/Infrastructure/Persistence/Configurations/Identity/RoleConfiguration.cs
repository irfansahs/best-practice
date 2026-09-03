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
        builder.Property(x => x.OrganizationPath).HasMaxLength(450);
        builder.Property(x => x.IsSystemRole).IsRequired();
        builder.Property(x => x.AllowedClients).HasConversion<int>().IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasMany(x => x.RolePermissions)
            .WithOne()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.RolePermissions).HasField("_rolePermissions");
        builder.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique();
        builder.HasIndex(x => x.OrganizationId);
    }
}
