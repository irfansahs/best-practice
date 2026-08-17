using Application.Abstractions.Localization;
using FluentValidation;

namespace Application.Identity.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator(ITranslator translator)
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(_ => translator["Identity.RefreshToken.NotFound"]);
    }
}
