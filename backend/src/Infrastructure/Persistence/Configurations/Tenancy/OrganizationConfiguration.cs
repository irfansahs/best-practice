using Domain.Tenancy;
using Domain.Tenancy.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tenancy;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations", Schemas.Tenancy);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(Organization.MaxNameLength).IsRequired();
        builder.Property(x => x.Slug)
            .HasConversion(s => s.Value, v => OrganizationSlug.Create(v).Value)
            .HasMaxLength(OrganizationSlug.MaxLength)
            .IsRequired();
        builder.Property(x => x.Path).HasMaxLength(Organization.MaxPathLength).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.ContactEmail)
            .HasConversion(
                e => e == null ? null : e.Value,
                v => string.IsNullOrEmpty(v) ? null : Domain.Identity.ValueObjects.Email.Create(v).Value)
            .HasMaxLength(Domain.Identity.ValueObjects.Email.MaxLength);
        builder.Property(x => x.TimeZoneId).HasMaxLength(Organization.MaxTimeZoneLength).IsRequired();
        builder.Property(x => x.DefaultCulture).HasMaxLength(Organization.MaxCultureLength).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.Path);
        builder.HasIndex(x => x.ParentId);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => new { x.Type, x.Status });
    }
}
