using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Identity.Features.Auth.Commands.SwitchOrganization;

public sealed class SwitchOrganizationCommandValidator : AbstractValidator<SwitchOrganizationCommand>
{
    public SwitchOrganizationCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.OrganizationId).NotEmpty().WithMessage(_ => translator["Validation.Required"]);
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(_ => translator["Identity.RefreshToken.NotFound"]);
    }
}
