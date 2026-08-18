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
}
