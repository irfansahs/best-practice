using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tenancy;

internal sealed class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Memberships", Schemas.Tenancy);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OrganizationPath).HasMaxLength(Organization.MaxPathLength).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(Membership.MaxTitleLength);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.JoinedAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasOne<Domain.Identity.User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Roles).WithOne().HasForeignKey(x => x.MembershipId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Overrides).WithOne().HasForeignKey(x => x.MembershipId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Roles).HasField("_roles");
        builder.Navigation(x => x.Overrides).HasField("_overrides");
        builder.HasIndex(x => new { x.UserId, x.OrganizationId }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => new { x.UserId, x.IsPrimary });
    }
}
