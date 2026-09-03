using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IAppDbContext db,
    ITokenService tokenService,
    IPermissionResolver permissionResolver,
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

        var user = await db.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId, cancellationToken);
        if (user is null) return IdentityErrors.UserNotFound;

        var trackedToken = user.RefreshTokens.FirstOrDefault(t => t.Id == storedToken.Id) ?? storedToken;

        if (trackedToken.IsRevoked)
        {
            user.RevokeRefreshTokenFamily(trackedToken.FamilyId, now, RefreshTokenRevokeReason.ReuseDetected);
            return IdentityErrors.RefreshTokenReuseDetected;
        }

        if (trackedToken.IsExpired(now)) return IdentityErrors.RefreshTokenExpired;

        var organization = await db.Organizations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == trackedToken.OrganizationId && !o.IsDeleted, cancellationToken);
        if (organization is null) return TenancyErrors.OrganizationNotFound;
        if (organization.Status == OrganizationStatus.Suspended) return TenancyErrors.OrganizationSuspended;
        if (!organization.IsActive) return TenancyErrors.OrganizationInactive;

        PermissionSet permissions;
        if (trackedToken.IsImpersonating)
        {
            var impersonationSession = await AuthSessionFactory.CreateAsync(
                db, permissionResolver, user, null, trackedToken.ClientType, impersonating: false, cancellationToken);
            var canImpersonate = impersonationSession.IsSuccess
                && impersonationSession.Value.Permissions.Allows(Security.Permissions.Tenancy.Organizations.Impersonate, PermissionScope.Global);
            if (!canImpersonate) return TenancyErrors.ImpersonationForbidden;
            permissions = await permissionResolver.ResolveAsync(user.Id, organization.Id, cancellationToken);
            if (permissions.Grants.Count == 0)
                permissions = impersonationSession.Value.Permissions;
        }
        else
        {
            var sessionResult = await AuthSessionFactory.CreateAsync(
                db, permissionResolver, user, organization.Id, trackedToken.ClientType, false, cancellationToken);
            if (sessionResult.IsFailure) return sessionResult.Error;
            permissions = sessionResult.Value.Permissions;
        }

        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = tokenService.HashRefreshToken(newRefreshToken);
        var newTokenId = Guid.NewGuid();
        var refreshExpiresAt = trackedToken.IsImpersonating
            ? tokenService.GetImpersonationAccessTokenExpiresAt(now).AddDays(1)
            : tokenService.GetRefreshTokenExpiresAt(now);

        trackedToken.Revoke(now, newTokenId, RefreshTokenRevokeReason.Rotated);
        user.IssueRefreshToken(
            newTokenId,
            newRefreshTokenHash,
            refreshExpiresAt,
            now,
            organization.Id,
            trackedToken.FamilyId,
            trackedToken.ClientType,
            trackedToken.IsImpersonating,
            trackedToken.DeviceId,
            trackedToken.DeviceName,
            trackedToken.CreatedByIp);
        db.RefreshTokens.Add(user.RefreshTokens.Last());

        var (accessToken, accessExpiresAt) = tokenService.GenerateAccessToken(
            new AccessTokenContext(user, organization, permissions, trackedToken.ClientType, trackedToken.IsImpersonating));

        return new RefreshTokenResponse(accessToken, newRefreshToken, accessExpiresAt);
    }
}
