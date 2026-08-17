using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Identity.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage(_ => translator["Identity.PasswordHash.Required"]);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).WithMessage(_ => translator["Identity.PasswordHash.Required"]);
    }
}
