using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Tenancy.Features.Organizations.Commands.UpdateOrganization;

public sealed class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
        RuleFor(x => x.Name).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
    }
}
