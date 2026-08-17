using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Catalog.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Sku).NotEmpty().WithMessage(_ => translator["Catalog.Sku.Required"]);
        RuleFor(x => x.Currency).NotEmpty().WithMessage(_ => translator["Catalog.Money.CurrencyRequired"]);
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage(_ => translator["Catalog.Category.IdRequired"]);
        RuleFor(x => x.LanguageId).NotEmpty().WithMessage(_ => translator["Catalog.Translation.LanguageRequired"]);
        RuleFor(x => x.Name).NotEmpty().WithMessage(_ => translator["Catalog.Translation.NameRequired"]);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage(_ => translator["Catalog.Money.AmountInvalid"]);
    }
}
