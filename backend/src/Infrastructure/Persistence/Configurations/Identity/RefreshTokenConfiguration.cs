using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", Schemas.Identity);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.FamilyId).IsRequired();
        builder.Property(x => x.ClientType).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(x => x.IsImpersonating).IsRequired();
        builder.Property(x => x.RevokedReason).HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.DeviceId).HasMaxLength(RefreshToken.MaxDeviceIdLength);
        builder.Property(x => x.DeviceName).HasMaxLength(RefreshToken.MaxDeviceNameLength);
        builder.Property(x => x.CreatedByIp).HasMaxLength(RefreshToken.MaxIpAddressLength);
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.FamilyId);
        builder.HasIndex(x => x.OrganizationId);
    }
}
