using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.Configuration;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IPermissionResolver permissionResolver,
    TimeProvider timeProvider,
    IOptions<LockoutOptions> lockoutOptions) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Domain.Identity.ValueObjects.Email.Create(request.Email);
        if (emailResult.IsFailure) return IdentityErrors.InvalidCredentials;

        var clientResult = ClientTypeParser.Parse(request.ClientType);
        if (clientResult.IsFailure) return clientResult.Error;
        var clientType = clientResult.Value;

        var now = timeProvider.GetUtcNow();
        var lockout = lockoutOptions.Value;

        var tracked = await db.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == emailResult.Value, cancellationToken);

        if (tracked is null || !tracked.IsActive) return IdentityErrors.InvalidCredentials;
        if (tracked.IsLockoutActive(now)) return IdentityErrors.UserAlreadyLocked;

        if (!passwordHasher.Verify(request.Password, tracked.PasswordHash.Value))
        {
            tracked.RecordFailedLogin(
                request.IpAddress,
                now,
                lockout.MaxFailedAttempts,
                TimeSpan.FromMinutes(lockout.LockoutMinutes),
                request.OrganizationId,
                clientType);
            db.LoginAttempts.Add(tracked.LoginAttempts.Last());
            return IdentityErrors.InvalidCredentials;
        }

        var session = await AuthSessionFactory.CreateAsync(
            db,
            permissionResolver,
            tracked,
            request.OrganizationId,
            clientType,
            impersonating: false,
            cancellationToken);

        if (session.IsFailure)
            return session.Error;

        tracked.RecordSuccessfulLogin(request.IpAddress, now, session.Value.Organization.Id, clientType);
        db.LoginAttempts.Add(tracked.LoginAttempts.Last());

        var tokens = AuthTokenIssuer.Issue(
            tokenService,
            tracked,
            session.Value,
            now,
            familyId: Guid.NewGuid(),
            request.DeviceId,
            request.DeviceName,
            request.IpAddress);

        db.RefreshTokens.Add(tracked.RefreshTokens.Last());
        return tokens;
    }
}
