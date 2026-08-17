using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.Abstractions.Time;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IClock clock) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Domain.Identity.ValueObjects.Email.Create(request.Email);
        if (emailResult.IsFailure) return IdentityErrors.InvalidCredentials;

        var now = clock.UtcNow;

        var tracked = await db.Users
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Email == emailResult.Value, cancellationToken);

        if (tracked is null || !tracked.IsActive) return IdentityErrors.InvalidCredentials;
        if (tracked.IsLockoutActive(now)) return IdentityErrors.UserAlreadyLocked;

        if (!passwordHasher.Verify(request.Password, tracked.PasswordHash.Value))
        {
            tracked.RecordFailedLogin(request.IpAddress, now);
            db.LoginAttempts.Add(tracked.LoginAttempts.Last());
            return IdentityErrors.InvalidCredentials;
        }

        tracked.RecordSuccessfulLogin(request.IpAddress, now);
        db.LoginAttempts.Add(tracked.LoginAttempts.Last());

        var (accessToken, accessExpiresAt) = tokenService.GenerateAccessToken(tracked);
        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashRefreshToken(refreshToken);
        var refreshExpiresAt = tokenService.GetRefreshTokenExpiresAt(now);
        tracked.IssueRefreshToken(Guid.NewGuid(), refreshTokenHash, refreshExpiresAt, now);
        db.RefreshTokens.Add(tracked.RefreshTokens.Last());

        return new LoginResponse(accessToken, refreshToken, accessExpiresAt);
    }
}
