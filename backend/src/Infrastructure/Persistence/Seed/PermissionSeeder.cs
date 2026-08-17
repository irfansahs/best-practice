using Domain.Identity;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class PermissionSeeder
{
    public static readonly Guid AdminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333301");

    private static readonly (Guid Id, string Code, string Description)[] PermissionDefinitions =
    [
        (Guid.Parse("44444444-4444-4444-4444-444444444401"), Permissions.Catalog.Products.Read, "Read products"),
        (Guid.Parse("44444444-4444-4444-4444-444444444402"), Permissions.Catalog.Products.Create, "Create products"),
        (Guid.Parse("44444444-4444-4444-4444-444444444403"), Permissions.Catalog.Products.Update, "Update products"),
        (Guid.Parse("44444444-4444-4444-4444-444444444404"), Permissions.Catalog.Products.Delete, "Delete products"),
        (Guid.Parse("44444444-4444-4444-4444-444444444405"), Permissions.Catalog.Categories.Read, "Read categories"),
        (Guid.Parse("44444444-4444-4444-4444-444444444408"), Permissions.Catalog.Categories.Create, "Create categories"),
        (Guid.Parse("44444444-4444-4444-4444-444444444409"), Permissions.Catalog.Categories.Update, "Update categories"),
        (Guid.Parse("44444444-4444-4444-4444-444444444410"), Permissions.Catalog.Categories.Delete, "Delete categories"),
        (Guid.Parse("44444444-4444-4444-4444-444444444406"), Permissions.Identity.Users.Read, "Read users"),
        (Guid.Parse("44444444-4444-4444-4444-444444444407"), Permissions.Localization.Translations.Manage, "Manage translations")
    ];

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Permissions.AnyAsync(cancellationToken))
        {
            await EnsureMissingPermissionsAsync(context, cancellationToken);
            return;
        }

        var permissions = PermissionDefinitions
            .Select(p => Permission.Create(p.Id, p.Code, p.Description).Value)
            .ToList();

        var adminRole = Role.Create(AdminRoleId, "Admin", "System administrator").Value;
        foreach (var permission in permissions)
            adminRole.GrantPermission(permission);

        context.Permissions.AddRange(permissions);
        context.Roles.Add(adminRole);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureMissingPermissionsAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var existingCodes = await context.Permissions.AsNoTracking()
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var missing = PermissionDefinitions
            .Where(p => !existingCodes.Contains(p.Code, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (missing.Count == 0) return;

        var adminRole = await context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == AdminRoleId, cancellationToken);

        foreach (var definition in missing)
        {
            var permission = Permission.Create(definition.Id, definition.Code, definition.Description).Value;
            context.Permissions.Add(permission);
            adminRole?.GrantPermission(permission);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
