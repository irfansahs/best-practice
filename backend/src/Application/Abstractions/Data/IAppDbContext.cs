using Domain.Catalog;
using Domain.Identity;
using Domain.Localization;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<LoginAttempt> LoginAttempts { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductTranslation> ProductTranslations { get; }
    DbSet<Category> Categories { get; }
    DbSet<CategoryTranslation> CategoryTranslations { get; }
    DbSet<Language> Languages { get; }
    DbSet<TranslationEntry> TranslationEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
