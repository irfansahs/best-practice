using SharedKernel.Results;

namespace Domain.Catalog;

public static class CatalogErrors
{
    public static Error ProductNotFound => Error.NotFound("Catalog.Product.NotFound", string.Empty);
    public static Error ProductIdRequired => Error.Validation("Catalog.Product.IdRequired", string.Empty);
    public static Error ProductInactive => Error.Conflict("Catalog.Product.Inactive", string.Empty);
    public static Error SkuAlreadyExists => Error.Conflict("Catalog.Product.SkuAlreadyExists", string.Empty);

    public static Error CategoryNotFound => Error.NotFound("Catalog.Category.NotFound", string.Empty);
    public static Error CategoryIdRequired => Error.Validation("Catalog.Category.IdRequired", string.Empty);
    public static Error CategoryHasProducts => Error.Conflict("Catalog.Category.HasProducts", string.Empty);
    public static Error CategoryParentInvalid => Error.Validation("Catalog.Category.ParentInvalid", string.Empty);

    public static Error TranslationLanguageRequired => Error.Validation("Catalog.Translation.LanguageRequired", string.Empty);
    public static Error TranslationNameRequired => Error.Validation("Catalog.Translation.NameRequired", string.Empty);
    public static Error TranslationNotFound => Error.NotFound("Catalog.Translation.NotFound", string.Empty);

    public static Error SkuRequired => Error.Validation("Catalog.Sku.Required", string.Empty);
    public static Error SkuInvalid => Error.Validation("Catalog.Sku.Invalid", string.Empty);
    public static Error SkuTooLong => Error.Validation("Catalog.Sku.TooLong", string.Empty);

    public static Error SlugRequired => Error.Validation("Catalog.Slug.Required", string.Empty);
    public static Error SlugInvalid => Error.Validation("Catalog.Slug.Invalid", string.Empty);
    public static Error SlugTooLong => Error.Validation("Catalog.Slug.TooLong", string.Empty);

    public static Error MoneyAmountInvalid => Error.Validation("Catalog.Money.AmountInvalid", string.Empty);
    public static Error MoneyCurrencyRequired => Error.Validation("Catalog.Money.CurrencyRequired", string.Empty);
    public static Error MoneyCurrencyInvalid => Error.Validation("Catalog.Money.CurrencyInvalid", string.Empty);
    public static Error PriceUnchanged => Error.Validation("Catalog.Money.PriceUnchanged", string.Empty);
}
