using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

internal sealed class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.ToTable("LoginAttempts", Schemas.Identity);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(LoginAttempt.MaxEmailLength).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(LoginAttempt.MaxIpAddressLength);
        builder.Property(x => x.AttemptedAt).IsRequired();
        builder.Property(x => x.ClientType).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.AttemptedAt);
        builder.HasIndex(x => x.OrganizationId);
    }
}
