using Domain.Identity;

namespace Application.Security;

public static class Permissions
{
    public static class Catalog
    {
        public static class Products
        {
            public const string Read = "catalog.products.read";
            public const string Create = "catalog.products.create";
            public const string Update = "catalog.products.update";
            public const string Delete = "catalog.products.delete";
        }

        public static class Categories
        {
            public const string Read = "catalog.categories.read";
            public const string Create = "catalog.categories.create";
            public const string Update = "catalog.categories.update";
            public const string Delete = "catalog.categories.delete";
        }
    }

    public static class Identity
    {
        public static class Users
        {
            public const string Read = "identity.users.read";
            public const string Manage = "identity.users.manage";
        }
    }

    public static class Localization
    {
        public const string Manage = "localization.translations.manage";
        public const string Read = "localization.translations.read";

        public static class Translations
        {
            public const string Manage = "localization.translations.manage";
            public const string Read = "localization.translations.read";
        }
    }

    public static class Tenancy
    {
        public static class Organizations
        {
            public const string Read = "tenancy.organizations.read";
            public const string Create = "tenancy.organizations.create";
            public const string Update = "tenancy.organizations.update";
            public const string Delete = "tenancy.organizations.delete";
            public const string Impersonate = "tenancy.organizations.impersonate";
        }

        public static class Members
        {
            public const string Read = "tenancy.members.read";
            public const string Manage = "tenancy.members.manage";
        }

        public static class Roles
        {
            public const string Read = "tenancy.roles.read";
            public const string Manage = "tenancy.roles.manage";
        }

        public static class PermissionsCatalog
        {
            public const string Read = "tenancy.permissions.read";
        }
    }

    public static class Aquaculture
    {
        public static class Sensors
        {
            public const string Read = "aquaculture.sensors.read";
            public const string Edit = "aquaculture.sensors.edit";
            public const string Delete = "aquaculture.sensors.delete";
        }

        public static class Facilities
        {
            public const string Read = "aquaculture.facilities.read";
            public const string Manage = "aquaculture.facilities.manage";
        }

        public static class Reports
        {
            public const string Export = "aquaculture.reports.export";
        }
    }

    public static PermissionScope DefaultMaxScope(string code)
    {
        if (code == Tenancy.Organizations.Impersonate) return PermissionScope.Global;
        if (code.StartsWith("tenancy.", StringComparison.Ordinal)) return PermissionScope.Subtree;
        if (code.StartsWith("identity.", StringComparison.Ordinal)) return PermissionScope.Subtree;
        if (code.StartsWith("localization.", StringComparison.Ordinal)) return PermissionScope.Global;
        return PermissionScope.Subtree;
    }

    public static bool IsPlatformOnly(string code) =>
        code == Tenancy.Organizations.Impersonate || code.StartsWith("localization.", StringComparison.Ordinal);
}
