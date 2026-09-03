using Application.Security;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public sealed class PermissionSeeder
{
    public static readonly Guid PlatformAdminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333301");
    public static readonly Guid OperatorAdminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333302");
    public static readonly Guid SupplierAdminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333303");
    public static readonly Guid MemberRoleId = Guid.Parse("33333333-3333-3333-3333-333333333304");

    public static readonly Guid AdminRoleId = PlatformAdminRoleId;

    private static readonly (Guid Id, string Code, string Description, PermissionScope MaxScope, bool PlatformOnly)[] PermissionDefinitions =
    [
        (Guid.Parse("44444444-4444-4444-4444-444444444401"), Permissions.Catalog.Products.Read, "Read products", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444402"), Permissions.Catalog.Products.Create, "Create products", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444403"), Permissions.Catalog.Products.Update, "Update products", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444404"), Permissions.Catalog.Products.Delete, "Delete products", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444405"), Permissions.Catalog.Categories.Read, "Read categories", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444408"), Permissions.Catalog.Categories.Create, "Create categories", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444409"), Permissions.Catalog.Categories.Update, "Update categories", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444410"), Permissions.Catalog.Categories.Delete, "Delete categories", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444406"), Permissions.Identity.Users.Read, "Read users", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444411"), Permissions.Identity.Users.Manage, "Manage users", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444407"), Permissions.Localization.Translations.Manage, "Manage translations", PermissionScope.Global, true),
        (Guid.Parse("44444444-4444-4444-4444-444444444412"), Permissions.Localization.Translations.Read, "Read translations", PermissionScope.Global, true),
        (Guid.Parse("44444444-4444-4444-4444-444444444420"), Permissions.Tenancy.Organizations.Read, "Read organizations", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444421"), Permissions.Tenancy.Organizations.Create, "Create organizations", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444422"), Permissions.Tenancy.Organizations.Update, "Update organizations", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444423"), Permissions.Tenancy.Organizations.Delete, "Delete organizations", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444424"), Permissions.Tenancy.Organizations.Impersonate, "Impersonate organizations", PermissionScope.Global, true),
        (Guid.Parse("44444444-4444-4444-4444-444444444425"), Permissions.Tenancy.Members.Read, "Read members", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444426"), Permissions.Tenancy.Members.Manage, "Manage members", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444427"), Permissions.Tenancy.Roles.Read, "Read roles", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444428"), Permissions.Tenancy.Roles.Manage, "Manage roles", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444429"), Permissions.Tenancy.PermissionsCatalog.Read, "Read permission catalog", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444430"), Permissions.Aquaculture.Sensors.Read, "Read sensors", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444431"), Permissions.Aquaculture.Sensors.Edit, "Edit sensors", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444432"), Permissions.Aquaculture.Sensors.Delete, "Delete sensors", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444433"), Permissions.Aquaculture.Facilities.Read, "Read facilities", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444434"), Permissions.Aquaculture.Facilities.Manage, "Manage facilities", PermissionScope.Subtree, false),
        (Guid.Parse("44444444-4444-4444-4444-444444444435"), Permissions.Aquaculture.Reports.Export, "Export reports", PermissionScope.Subtree, false)
    ];

    public async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        var permissions = await EnsurePermissionsAsync(context, cancellationToken);
        var byCode = permissions.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

        await EnsureRoleAsync(
            context,
            PlatformAdminRoleId,
            "PlatformAdmin",
            "Ranna platform administrator",
            ClientTypes.Web,
            permissions.Select(p => (p, p.MaxScope)),
            cancellationToken);

        await EnsureRoleAsync(
            context,
            OperatorAdminRoleId,
            "OperatorAdmin",
            "Operator administrator",
            ClientTypes.All,
            Grants(byCode, PermissionScope.Subtree,
                Permissions.Catalog.Products.Read, Permissions.Catalog.Products.Create, Permissions.Catalog.Products.Update, Permissions.Catalog.Products.Delete,
                Permissions.Catalog.Categories.Read, Permissions.Catalog.Categories.Create, Permissions.Catalog.Categories.Update, Permissions.Catalog.Categories.Delete,
                Permissions.Tenancy.Organizations.Read, Permissions.Tenancy.Organizations.Create, Permissions.Tenancy.Organizations.Update,
                Permissions.Tenancy.Members.Read, Permissions.Tenancy.Members.Manage,
                Permissions.Tenancy.Roles.Read, Permissions.Tenancy.Roles.Manage,
                Permissions.Tenancy.PermissionsCatalog.Read,
                Permissions.Identity.Users.Read,
                Permissions.Aquaculture.Sensors.Read, Permissions.Aquaculture.Sensors.Edit, Permissions.Aquaculture.Sensors.Delete,
                Permissions.Aquaculture.Facilities.Read, Permissions.Aquaculture.Facilities.Manage,
                Permissions.Aquaculture.Reports.Export),
            cancellationToken);

        await EnsureRoleAsync(
            context,
            SupplierAdminRoleId,
            "SupplierAdmin",
            "Supplier administrator",
            ClientTypes.All,
            Grants(byCode, PermissionScope.Organization,
                Permissions.Catalog.Products.Read, Permissions.Catalog.Products.Create, Permissions.Catalog.Products.Update, Permissions.Catalog.Products.Delete,
                Permissions.Catalog.Categories.Read, Permissions.Catalog.Categories.Create, Permissions.Catalog.Categories.Update, Permissions.Catalog.Categories.Delete,
                Permissions.Tenancy.Organizations.Read,
                Permissions.Tenancy.Members.Read, Permissions.Tenancy.Members.Manage,
                Permissions.Tenancy.Roles.Read,
                Permissions.Identity.Users.Read,
                Permissions.Aquaculture.Sensors.Read, Permissions.Aquaculture.Sensors.Edit,
                Permissions.Aquaculture.Facilities.Read, Permissions.Aquaculture.Facilities.Manage,
                Permissions.Aquaculture.Reports.Export),
            cancellationToken);

        await EnsureRoleAsync(
            context,
            MemberRoleId,
            "Member",
            "Standard member",
            ClientTypes.All,
            Grants(byCode, PermissionScope.Own,
                Permissions.Catalog.Products.Read,
                Permissions.Catalog.Categories.Read,
                Permissions.Aquaculture.Sensors.Read,
                Permissions.Aquaculture.Facilities.Read),
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<(Permission Permission, PermissionScope Scope)> Grants(
        IReadOnlyDictionary<string, Permission> byCode,
        PermissionScope scope,
        params string[] codes)
    {
        foreach (var code in codes)
        {
            if (byCode.TryGetValue(code, out var permission))
                yield return (permission, scope);
        }
    }

    private static async Task<List<Permission>> EnsurePermissionsAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var existing = await context.Permissions.ToListAsync(cancellationToken);
        var byCode = existing.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

        foreach (var definition in PermissionDefinitions)
        {
            if (byCode.TryGetValue(definition.Code, out var permission))
            {
                permission.SyncCatalog(definition.MaxScope, definition.PlatformOnly);
                continue;
            }

            permission = Permission.Create(
                definition.Id,
                definition.Code,
                definition.Description,
                maxScope: definition.MaxScope,
                isPlatformOnly: definition.PlatformOnly).Value;
            context.Permissions.Add(permission);
            existing.Add(permission);
            byCode[permission.Code] = permission;
        }

        return existing;
    }

    private static async Task EnsureRoleAsync(
        AppDbContext context,
        Guid roleId,
        string name,
        string description,
        ClientTypes allowedClients,
        IEnumerable<(Permission Permission, PermissionScope Scope)> grants,
        CancellationToken cancellationToken)
    {
        var role = await context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role is null)
        {
            role = Role.Create(roleId, name, description, isSystemRole: true, allowedClients: allowedClients).Value;
            context.Roles.Add(role);
        }
        else
        {
            role.EnsureSystem(name, description, allowedClients);
        }

        foreach (var (permission, scope) in grants)
        {
            var grant = role.GrantPermission(permission, scope);
            if (grant.IsFailure)
                throw new InvalidOperationException($"Failed to grant '{permission.Code}' to role '{name}': {grant.Error.Code}");
        }
    }
}
