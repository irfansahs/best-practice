using Application.Abstractions.Data;
using Application.Abstractions.Tenancy;
using Domain.Catalog;
using Domain.Identity;
using Domain.Localization;
using Domain.Tenancy;
using Infrastructure.Persistence.Conventions;
using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Auditing;
using Domain.Abstractions;

namespace Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IAppDbContext, IUnitOfWork
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public string TenantPath => _tenantContext.OrganizationPath;

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<MembershipRole> MembershipRoles => Set<MembershipRole>();
    public DbSet<PermissionOverride> PermissionOverrides => Set<PermissionOverride>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductTranslation> ProductTranslations => Set<ProductTranslation>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryTranslation> CategoryTranslations => Set<CategoryTranslation>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<TranslationEntry> TranslationEntries => Set<TranslationEntry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        DateTimeOffsetUtcConvention.Apply(modelBuilder);
        ApplyTenantFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>().HasQueryFilter(o => !o.IsDeleted && o.Path.StartsWith(TenantPath));
        modelBuilder.Entity<Membership>().HasQueryFilter(m => !m.IsDeleted && m.OrganizationPath.StartsWith(TenantPath));
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted && p.OrganizationPath.StartsWith(TenantPath));
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted && c.OrganizationPath.StartsWith(TenantPath));
    }
}
