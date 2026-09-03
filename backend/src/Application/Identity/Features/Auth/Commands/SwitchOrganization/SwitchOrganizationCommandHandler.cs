using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.Identity.Features.Auth.Commands.Login;
using Application.Security;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.SwitchOrganization;

public sealed class SwitchOrganizationCommandHandler(
    IAppDbContext db,
    ITokenService tokenService,
    IPermissionResolver permissionResolver,
    TimeProvider timeProvider) : IRequestHandler<SwitchOrganizationCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(SwitchOrganizationCommand request, CancellationToken cancellationToken)
    {
        var clientResult = ClientTypeParser.Parse(request.ClientType);
        if (clientResult.IsFailure) return clientResult.Error;

        var now = timeProvider.GetUtcNow();
        var refreshTokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash, cancellationToken);
        if (storedToken is null) return IdentityErrors.RefreshTokenNotFound;
        if (storedToken.IsRevoked) return IdentityErrors.RefreshTokenReuseDetected;
        if (storedToken.IsExpired(now)) return IdentityErrors.RefreshTokenExpired;

        var user = await db.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId, cancellationToken);
        if (user is null || !user.IsActive) return IdentityErrors.UserNotFound;

        var target = await db.Organizations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == request.OrganizationId && !o.IsDeleted, cancellationToken);
        if (target is null) return TenancyErrors.OrganizationNotFound;
        if (!target.IsActive) return TenancyErrors.OrganizationInactive;

        var membership = await db.Memberships
            .IgnoreQueryFilters()
            .Include(m => m.Roles).ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(m => m.UserId == user.Id && m.OrganizationId == target.Id && !m.IsDeleted, cancellationToken);

        var impersonating = false;
        PermissionSet permissions;

        if (membership is { IsActive: true })
        {
            if (!membership.AllowsClient(clientResult.Value))
                return IdentityErrors.ClientTypeNotAllowed;
            permissions = await permissionResolver.ResolveAsync(user.Id, target.Id, cancellationToken);
        }
        else
        {
            var home = await AuthSessionFactory.CreateAsync(
                db, permissionResolver, user, storedToken.OrganizationId, clientResult.Value, false, cancellationToken);
            if (home.IsFailure || !home.Value.Permissions.Allows(Permissions.Tenancy.Organizations.Impersonate, PermissionScope.Global))
                return TenancyErrors.ImpersonationForbidden;
            impersonating = true;
            permissions = home.Value.Permissions;
        }

        user.RevokeRefreshTokenFamily(storedToken.FamilyId, now, RefreshTokenRevokeReason.Rotated);

        var session = new AuthSession(target, permissions, impersonating, clientResult.Value);
        var tokens = AuthTokenIssuer.Issue(
            tokenService,
            user,
            session,
            now,
            Guid.NewGuid(),
            storedToken.DeviceId,
            storedToken.DeviceName,
            storedToken.CreatedByIp);

        db.RefreshTokens.Add(user.RefreshTokens.Last());
        return tokens;
    }
}
