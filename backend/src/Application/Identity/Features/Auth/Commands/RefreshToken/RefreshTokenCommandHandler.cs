using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IAppDbContext db,
    ITokenService tokenService,
    TimeProvider timeProvider) : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) return IdentityErrors.RefreshTokenNotFound;

        var now = timeProvider.GetUtcNow();
        var refreshTokenHash = tokenService.HashRefreshToken(request.RefreshToken);

        var storedToken = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash, cancellationToken);

        if (storedToken is null) return IdentityErrors.RefreshTokenNotFound;
        if (storedToken.IsRevoked) return IdentityErrors.RefreshTokenReuseDetected;
        if (storedToken.IsExpired(now)) return IdentityErrors.RefreshTokenExpired;

        var user = await db.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId, cancellationToken);
        if (user is null) return IdentityErrors.UserNotFound;

        // Re-attach tracked token from user graph when available
        var trackedToken = user.RefreshTokens.FirstOrDefault(t => t.Id == storedToken.Id) ?? storedToken;

        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = tokenService.HashRefreshToken(newRefreshToken);
        var newTokenId = Guid.NewGuid();
        var refreshExpiresAt = tokenService.GetRefreshTokenExpiresAt(now);

        trackedToken.Revoke(now, newTokenId);
        user.IssueRefreshToken(newTokenId, newRefreshTokenHash, refreshExpiresAt, now);
        db.RefreshTokens.Add(user.RefreshTokens.Last());

        var (accessToken, accessExpiresAt) = tokenService.GenerateAccessToken(user);
        return new RefreshTokenResponse(accessToken, newRefreshToken, accessExpiresAt);
    }
}
