using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser,
    IPermissionResolver permissionResolver) : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return IdentityErrors.UserNotFound;

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

        if (user is null) return IdentityErrors.UserNotFound;

        var orgDtos = await OrganizationDirectory.ListForUserAsync(db, user.Id, currentUser, cancellationToken);

        IReadOnlyDictionary<string, int> permissions;
        OrganizationSummaryDto? active = null;

        if (currentUser.OrganizationId is { } activeOrgId)
        {
            active = orgDtos.FirstOrDefault(o => o.Id == activeOrgId);
            if (active is null)
            {
                var impersonated = await db.Organizations
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == activeOrgId && !o.IsDeleted, cancellationToken);
                if (impersonated is not null)
                    active = new OrganizationSummaryDto(
                        impersonated.Id,
                        impersonated.Name,
                        impersonated.Slug.Value,
                        impersonated.Type.ToString(),
                        impersonated.Path,
                        false);
            }

            permissions = currentUser.PermissionMap.ToDictionary(k => k.Key, v => (int)v.Value, StringComparer.OrdinalIgnoreCase);
            if (permissions.Count == 0)
            {
                var resolved = await permissionResolver.ResolveAsync(user.Id, activeOrgId, cancellationToken);
                permissions = resolved.Grants.ToDictionary(k => k.Key, v => (int)v.Value, StringComparer.OrdinalIgnoreCase);
            }
        }
        else
        {
            permissions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        return UserMapper.ToCurrentUserDto(user, permissions, active, orgDtos, currentUser.IsImpersonating);
    }
}
