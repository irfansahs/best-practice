using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Identity.ValueObjects;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class IdentitySeeder(IPasswordHasher passwordHasher)
{
    public static readonly Guid AdminUserId = Guid.Parse("66666666-6666-6666-6666-666666666601");
    public const string AdminEmail = "admin@local.dev";
    public const string AdminPassword = "ChangeMe123!";

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (!await context.Users.IgnoreQueryFilters().AnyAsync(u => u.Id == AdminUserId, cancellationToken))
        {
            var user = User.Register(
                AdminUserId,
                Email.Create(AdminEmail).Value,
                PasswordHash.Create(passwordHasher.Hash(AdminPassword)).Value,
                FullName.Create("System", "Administrator").Value).Value;

            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        await EnsureAdminMembershipAsync(context, cancellationToken);
    }

    private static async Task EnsureAdminMembershipAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var existing = await context.Memberships.IgnoreQueryFilters()
            .AnyAsync(m => m.UserId == AdminUserId && m.OrganizationId == OrganizationSeeder.RannaId && !m.IsDeleted, cancellationToken);
        if (existing) return;

        var adminRole = await context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == PermissionSeeder.PlatformAdminRoleId, cancellationToken);

        var ranna = await context.Organizations.IgnoreQueryFilters()
            .FirstAsync(o => o.Id == OrganizationSeeder.RannaId, cancellationToken);

        var membership = Membership.Create(
            Guid.Parse("77777777-7777-7777-7777-777777777701"),
            AdminUserId,
            ranna,
            isPrimary: true,
            joinedAt: DateTimeOffset.UtcNow,
            title: "Platform administrator").Value;

        membership.AssignRole(adminRole, DateTimeOffset.UtcNow);
        context.Memberships.Add(membership);
        await context.SaveChangesAsync(cancellationToken);
    }
}
