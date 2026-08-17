using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Catalog.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_ => translator["Catalog.Product.IdRequired"]);
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage(_ => translator["Catalog.Category.IdRequired"]);
        RuleFor(x => x.LanguageId).NotEmpty().WithMessage(_ => translator["Catalog.Translation.LanguageRequired"]);
        RuleFor(x => x.Name).NotEmpty().WithMessage(_ => translator["Catalog.Translation.NameRequired"]);
    }
}
