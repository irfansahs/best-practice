using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Security;
using Application.Caching;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Security;

public sealed class PermissionResolver(IAppDbContext db, ICacheService cache) : IPermissionResolver
{
    public async Task<PermissionSet> ResolveAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default)
    {
        var scopes = await cache.GetOrCreateAsync(
            CacheKeys.PermissionSet(userId, organizationId),
            async ct => await LoadScopesAsync(userId, organizationId, ct),
            tags: [CacheTags.Permissions],
            cancellationToken: cancellationToken);

        return PermissionSet.FromScopes(scopes);
    }

    private async Task<Dictionary<string, int>> LoadScopesAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken)
    {
        var membership = await db.Memberships
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(m => m.Roles)
                .ThenInclude(r => r.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(m => m.Overrides)
                .ThenInclude(o => o.Permission)
            .FirstOrDefaultAsync(
                m => m.UserId == userId && m.OrganizationId == organizationId && !m.IsDeleted,
                cancellationToken);

        if (membership is null || !membership.IsActive)
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        return membership.ResolvePermissions(DateTimeOffset.UtcNow)
            .Grants
            .ToDictionary(g => g.Key, g => (int)g.Value, StringComparer.OrdinalIgnoreCase);
    }
}
