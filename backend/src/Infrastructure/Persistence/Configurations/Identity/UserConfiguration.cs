using Domain.Identity;
using Domain.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", Schemas.Identity);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasConversion(e => e.Value, v => Email.Create(v).Value).HasMaxLength(Email.MaxLength).IsRequired();
        builder.Property(x => x.PasswordHash).HasConversion(p => p.Value, v => PasswordHash.Create(v).Value).HasMaxLength(PasswordHash.MaxLength).IsRequired();
        builder.OwnsOne(x => x.FullName, name =>
        {
            name.Property(n => n.FirstName).HasColumnName("FirstName").HasMaxLength(FullName.MaxPartLength).IsRequired();
            name.Property(n => n.LastName).HasColumnName("LastName").HasMaxLength(FullName.MaxPartLength).IsRequired();
        });
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.IsLockedOut).IsRequired();
        builder.Property(x => x.FailedLoginAttempts).IsRequired();
        builder.Property(x => x.SecurityStamp).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasMany(x => x.Roles).WithMany().UsingEntity(j => j.ToTable("UserRoles", Schemas.Identity));
        builder.HasMany(x => x.RefreshTokens).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.LoginAttempts).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Roles).HasField("_roles");
        builder.Navigation(x => x.RefreshTokens).HasField("_refreshTokens");
        builder.Navigation(x => x.LoginAttempts).HasField("_loginAttempts");
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
