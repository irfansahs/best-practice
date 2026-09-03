using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Identity.ValueObjects;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class IdentitySeeder(IPasswordHasher passwordHasher)
{
    public static readonly Guid AdminUserId = Guid.Parse("66666666-6666-6666-6666-666666666601");
    public static readonly Guid AquaCareUserId = Guid.Parse("66666666-6666-6666-6666-666666666602");
    public static readonly Guid SupplierUserId = Guid.Parse("66666666-6666-6666-6666-666666666603");

    public const string AdminEmail = "admin@local.dev";
    public const string AdminPassword = "ChangeMe123!";
    public const string AquaCareEmail = "aquacare@local.dev";
    public const string AquaCarePassword = "AquaCare123";
    public const string SupplierEmail = "supplier@local.dev";
    public const string SupplierPassword = "Supplier123";

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await EnsureUserAsync(
            context,
            AdminUserId,
            AdminEmail,
            AdminPassword,
            "System",
            "Administrator",
            cancellationToken);

        await EnsureUserAsync(
            context,
            AquaCareUserId,
            AquaCareEmail,
            AquaCarePassword,
            "AquaCare",
            "Operator",
            cancellationToken);

        await EnsureUserAsync(
            context,
            SupplierUserId,
            SupplierEmail,
            SupplierPassword,
            "Demo",
            "Supplier",
            cancellationToken);

        await EnsureMembershipAsync(
            context,
            Guid.Parse("77777777-7777-7777-7777-777777777701"),
            AdminUserId,
            OrganizationSeeder.RannaId,
            PermissionSeeder.PlatformAdminRoleId,
            "Platform administrator",
            cancellationToken);

        await EnsureMembershipAsync(
            context,
            Guid.Parse("77777777-7777-7777-7777-777777777702"),
            AquaCareUserId,
            OrganizationSeeder.AquaCareId,
            PermissionSeeder.OperatorAdminRoleId,
            "AquaCare operator",
            cancellationToken);

        await EnsureMembershipAsync(
            context,
            Guid.Parse("77777777-7777-7777-7777-777777777703"),
            SupplierUserId,
            OrganizationSeeder.DemoSupplierId,
            PermissionSeeder.SupplierAdminRoleId,
            "Supplier administrator",
            cancellationToken);
    }

    private async Task EnsureUserAsync(
        AppDbContext context,
        Guid userId,
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        var existing = await context.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (existing is not null)
        {
            if (!passwordHasher.Verify(password, existing.PasswordHash.Value))
                existing.ChangePassword(PasswordHash.Create(passwordHasher.Hash(password)).Value);

            if (existing.IsLockedOut)
                existing.Unlock();

            await context.SaveChangesAsync(cancellationToken);
            return;
        }

        var user = User.Register(
            userId,
            Email.Create(email).Value,
            PasswordHash.Create(passwordHasher.Hash(password)).Value,
            FullName.Create(firstName, lastName).Value).Value;

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureMembershipAsync(
        AppDbContext context,
        Guid membershipId,
        Guid userId,
        Guid organizationId,
        Guid roleId,
        string title,
        CancellationToken cancellationToken)
    {
        var exists = await context.Memberships.IgnoreQueryFilters()
            .AnyAsync(m => m.UserId == userId && m.OrganizationId == organizationId && !m.IsDeleted, cancellationToken);
        if (exists) return;

        var role = await context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == roleId, cancellationToken);

        var organization = await context.Organizations.IgnoreQueryFilters()
            .FirstAsync(o => o.Id == organizationId, cancellationToken);

        var membership = Membership.Create(
            membershipId,
            userId,
            organization,
            isPrimary: true,
            joinedAt: DateTimeOffset.UtcNow,
            title: title).Value;

        membership.AssignRole(role, DateTimeOffset.UtcNow);
        context.Memberships.Add(membership);
        await context.SaveChangesAsync(cancellationToken);
    }
}
