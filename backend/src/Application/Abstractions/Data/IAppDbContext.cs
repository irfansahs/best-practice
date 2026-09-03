using Domain.Catalog;
using Domain.Identity;
using Domain.Localization;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<LoginAttempt> LoginAttempts { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Membership> Memberships { get; }
    DbSet<MembershipRole> MembershipRoles { get; }
    DbSet<PermissionOverride> PermissionOverrides { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductTranslation> ProductTranslations { get; }
    DbSet<Category> Categories { get; }
    DbSet<CategoryTranslation> CategoryTranslations { get; }
    DbSet<Language> Languages { get; }
    DbSet<TranslationEntry> TranslationEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
