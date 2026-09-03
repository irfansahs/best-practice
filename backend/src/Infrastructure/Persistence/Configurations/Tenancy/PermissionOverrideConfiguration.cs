using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tenancy;

internal sealed class PermissionOverrideConfiguration : IEntityTypeConfiguration<PermissionOverride>
{
    public void Configure(EntityTypeBuilder<PermissionOverride> builder)
    {
        builder.ToTable("PermissionOverrides", Schemas.Tenancy);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Effect).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(x => x.Scope).HasConversion<int>().IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(PermissionOverride.MaxReasonLength);
        builder.HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.MembershipId, x.PermissionId }).IsUnique();
    }
}
