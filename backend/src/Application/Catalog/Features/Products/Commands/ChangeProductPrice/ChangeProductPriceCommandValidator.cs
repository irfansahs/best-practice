using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Catalog.Features.Products.Commands.ChangeProductPrice;

public sealed class ChangeProductPriceCommandValidator : AbstractValidator<ChangeProductPriceCommand>
{
    public ChangeProductPriceCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_ => translator["Catalog.Product.IdRequired"]);
        RuleFor(x => x.Currency).NotEmpty().WithMessage(_ => translator["Catalog.Money.CurrencyRequired"]);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage(_ => translator["Catalog.Money.AmountInvalid"]);
    }
}
