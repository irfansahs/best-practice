using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Members.Commands.AddMember;

public sealed class AddMemberCommandHandler(IAppDbContext db, ICurrentUser currentUser, TimeProvider timeProvider)
    : IRequestHandler<AddMemberCommand, AddMemberResponse>
{
    public async Task<Result<AddMemberResponse>> Handle(AddMemberCommand request, CancellationToken cancellationToken)
    {
        var organization = await db.Organizations.FirstOrDefaultAsync(o => o.Id == request.OrganizationId, cancellationToken);
        if (organization is null) return TenancyErrors.OrganizationNotFound;

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return IdentityErrors.UserNotFound;

        var exists = await db.Memberships.IgnoreQueryFilters()
            .AnyAsync(m => m.UserId == request.UserId && m.OrganizationId == request.OrganizationId && !m.IsDeleted, cancellationToken);
        if (exists) return TenancyErrors.MembershipAlreadyExists;

        var roles = await db.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (roles.Count != request.RoleIds.Distinct().Count()) return IdentityErrors.RoleNotFound;

        foreach (var role in roles)
        {
            if (!role.CanBeAssignedTo(organization.Path))
                return TenancyErrors.RoleNotAssignableToOrganization;
        }

        var membership = Membership.Create(
            Guid.NewGuid(),
            user.Id,
            organization,
            request.IsPrimary,
            timeProvider.GetUtcNow(),
            request.Title,
            currentUser.UserId);

        if (membership.IsFailure) return membership.Error;

        foreach (var role in roles)
        {
            var assigned = membership.Value.AssignRole(role, timeProvider.GetUtcNow(), currentUser.UserId);
            if (assigned.IsFailure) return assigned.Error;
        }

        if (request.IsPrimary)
        {
            var others = await db.Memberships.IgnoreQueryFilters()
                .Where(m => m.UserId == user.Id && m.IsPrimary && !m.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var other in others)
                other.ClearPrimary();
        }

        db.Memberships.Add(membership.Value);
        return new AddMemberResponse(membership.Value.Id);
    }
}
