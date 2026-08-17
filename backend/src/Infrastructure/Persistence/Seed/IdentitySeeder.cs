using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class IdentitySeeder(IPasswordHasher passwordHasher)
{
    public static readonly Guid AdminUserId = Guid.Parse("66666666-6666-6666-6666-666666666601");
    public const string AdminEmail = "admin@local.dev";
    public const string AdminPassword = "ChangeMe123!";

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(cancellationToken)) return;

        var adminRole = await context.Roles
            .Include(r => r.Permissions)
            .FirstAsync(r => r.Id == PermissionSeeder.AdminRoleId, cancellationToken);

        var user = User.Register(
            AdminUserId,
            Email.Create(AdminEmail).Value,
            PasswordHash.Create(passwordHasher.Hash(AdminPassword)).Value,
            FullName.Create("System", "Administrator").Value).Value;

        user.AssignRole(adminRole);
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}
