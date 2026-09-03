using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(IAppDbContext db, ITokenService tokenService, TimeProvider timeProvider) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) return Unit.Value;

        var now = timeProvider.GetUtcNow();
        var refreshTokenHash = tokenService.HashRefreshToken(request.RefreshToken);

        var storedToken = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash, cancellationToken);

        storedToken?.Revoke(now, reason: Domain.Identity.RefreshTokenRevokeReason.Logout);

        return Unit.Value;
    }
}
