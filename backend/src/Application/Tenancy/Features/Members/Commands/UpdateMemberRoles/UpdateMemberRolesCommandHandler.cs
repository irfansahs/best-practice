using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Members.Commands.UpdateMemberRoles;

public sealed class UpdateMemberRolesCommandHandler(IAppDbContext db, ICurrentUser currentUser, TimeProvider timeProvider)
    : IRequestHandler<UpdateMemberRolesCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateMemberRolesCommand request, CancellationToken cancellationToken)
    {
        var membership = await db.Memberships
            .Include(m => m.Roles).ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(m => m.Id == request.MembershipId, cancellationToken);
        if (membership is null) return TenancyErrors.MembershipNotFound;

        var organization = await db.Organizations.FirstOrDefaultAsync(o => o.Id == membership.OrganizationId, cancellationToken);
        if (organization is null) return TenancyErrors.OrganizationNotFound;

        var roles = await db.Roles
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);
        if (roles.Count != request.RoleIds.Distinct().Count()) return IdentityErrors.RoleNotFound;

        foreach (var existing in membership.Roles.ToArray())
            membership.RemoveRole(existing.RoleId);

        foreach (var role in roles)
        {
            if (!role.CanBeAssignedTo(organization.Path))
                return TenancyErrors.RoleNotAssignableToOrganization;
            var assigned = membership.AssignRole(role, timeProvider.GetUtcNow(), currentUser.UserId);
            if (assigned.IsFailure) return assigned.Error;
        }

        return Unit.Value;
    }
}
