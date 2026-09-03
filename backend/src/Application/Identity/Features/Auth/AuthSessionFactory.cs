using Application.Abstractions.Data;
using Application.Abstractions.Security;
using Application.Identity.Features.Auth.Commands.Login;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth;

public sealed record AuthSession(
    Organization Organization,
    PermissionSet Permissions,
    bool IsImpersonating,
    ClientType ClientType);

public static class AuthSessionFactory
{
    public static async Task<Result<AuthSession>> CreateAsync(
        IAppDbContext db,
        IPermissionResolver permissionResolver,
        User user,
        Guid? organizationId,
        ClientType clientType,
        bool impersonating,
        CancellationToken cancellationToken)
    {
        var memberships = await db.Memberships
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(m => m.Roles)
                .ThenInclude(r => r.Role)
            .Where(m => m.UserId == user.Id && !m.IsDeleted)
            .ToListAsync(cancellationToken);

        var active = memberships.Where(m => m.Status == MembershipStatus.Active).ToList();
        Membership? membership;
        if (organizationId is { } requestedId)
            membership = active.FirstOrDefault(m => m.OrganizationId == requestedId);
        else
            membership = active.FirstOrDefault(m => m.IsPrimary) ?? active.FirstOrDefault();

        if (membership is null) return TenancyErrors.NoMembership;

        var organization = await db.Organizations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == membership.OrganizationId && !o.IsDeleted, cancellationToken);

        if (organization is null) return TenancyErrors.OrganizationNotFound;
        if (organization.Status == OrganizationStatus.Suspended) return TenancyErrors.OrganizationSuspended;
        if (!organization.IsActive) return TenancyErrors.OrganizationInactive;

        if (!membership.AllowsClient(clientType))
            return IdentityErrors.ClientTypeNotAllowed;

        var permissions = await permissionResolver.ResolveAsync(user.Id, organization.Id, cancellationToken);
        return new AuthSession(organization, permissions, impersonating, clientType);
    }
}

public static class AuthTokenIssuer
{
    public static LoginResponse Issue(
        ITokenService tokenService,
        User user,
        AuthSession session,
        DateTimeOffset now,
        Guid familyId,
        string? deviceId,
        string? deviceName,
        string? ipAddress)
    {
        var context = new AccessTokenContext(
            user,
            session.Organization,
            session.Permissions,
            session.ClientType,
            session.IsImpersonating);

        var (accessToken, accessExpiresAt) = tokenService.GenerateAccessToken(context);
        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashRefreshToken(refreshToken);
        var refreshExpiresAt = session.IsImpersonating
            ? tokenService.GetImpersonationAccessTokenExpiresAt(now).AddDays(1)
            : tokenService.GetRefreshTokenExpiresAt(now);

        user.IssueRefreshToken(
            Guid.NewGuid(),
            refreshTokenHash,
            refreshExpiresAt,
            now,
            session.Organization.Id,
            familyId,
            session.ClientType,
            session.IsImpersonating,
            deviceId,
            deviceName,
            ipAddress);

        return new LoginResponse(accessToken, refreshToken, accessExpiresAt);
    }
}
