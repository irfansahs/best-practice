using Application.Abstractions.Data;
using Application.Abstractions.Security;
using Application.Identity.Features.Auth.Queries.GetCurrentUser;
using Application.Security;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace Application.Identity.Features.Auth;

internal static class OrganizationDirectory
{
    public static async Task<IReadOnlyList<OrganizationSummaryDto>> ListForUserAsync(
        IAppDbContext db,
        Guid userId,
        ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        var memberships = await db.Memberships
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(m => m.UserId == userId && !m.IsDeleted && m.Status == MembershipStatus.Active)
            .ToListAsync(cancellationToken);

        var canImpersonate = currentUser.HasPermission(
            Permissions.Tenancy.Organizations.Impersonate,
            PermissionScope.Global);

        List<Organization> organizations;
        if (canImpersonate)
        {
            organizations = await db.Organizations
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(o => !o.IsDeleted && o.Status == OrganizationStatus.Active)
                .OrderBy(o => o.Path)
                .ToListAsync(cancellationToken);
        }
        else
        {
            var orgIds = memberships.Select(m => m.OrganizationId).ToArray();
            organizations = await db.Organizations
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(o => orgIds.Contains(o.Id) && !o.IsDeleted && o.Status == OrganizationStatus.Active)
                .OrderBy(o => o.Path)
                .ToListAsync(cancellationToken);
        }

        return organizations.Select(o =>
        {
            var membership = memberships.FirstOrDefault(m => m.OrganizationId == o.Id);
            return new OrganizationSummaryDto(
                o.Id,
                o.Name,
                o.Slug.Value,
                o.Type.ToString(),
                o.Path,
                membership?.IsPrimary ?? false);
        }).ToArray();
    }
}
