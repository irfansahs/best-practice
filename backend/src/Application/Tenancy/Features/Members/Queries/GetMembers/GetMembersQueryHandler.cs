using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Members.Queries.GetMembers;

public sealed class GetMembersQueryHandler(IAppDbContext db) : IRequestHandler<GetMembersQuery, IReadOnlyList<MemberListItemDto>>
{
    public async Task<Result<IReadOnlyList<MemberListItemDto>>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        var memberships = await db.Memberships
            .AsNoTracking()
            .Include(m => m.Roles).ThenInclude(r => r.Role)
            .Where(m => m.OrganizationId == request.OrganizationId)
            .ToListAsync(cancellationToken);

        var userIds = memberships.Select(m => m.UserId).Distinct().ToArray();
        var users = await db.Users.AsNoTracking().IgnoreQueryFilters()
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var userMap = users.ToDictionary(u => u.Id);
        IReadOnlyList<MemberListItemDto> result = memberships.Select(m =>
        {
            userMap.TryGetValue(m.UserId, out var user);
            return new MemberListItemDto(
                m.Id,
                m.UserId,
                user?.Email.Value ?? string.Empty,
                user?.FullName.DisplayName ?? string.Empty,
                m.Status.ToString(),
                m.IsPrimary,
                m.Title,
                m.Roles.Select(r => r.Role.Name).ToArray());
        }).ToArray();

        return Result<IReadOnlyList<MemberListItemDto>>.Success(result);
    }
}
