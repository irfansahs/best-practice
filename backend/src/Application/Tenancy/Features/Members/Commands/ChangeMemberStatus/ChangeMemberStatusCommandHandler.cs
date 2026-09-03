using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Members.Commands.ChangeMemberStatus;

public sealed class ChangeMemberStatusCommandHandler(IAppDbContext db) : IRequestHandler<ChangeMemberStatusCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ChangeMemberStatusCommand request, CancellationToken cancellationToken)
    {
        var membership = await db.Memberships.FirstOrDefaultAsync(m => m.Id == request.MembershipId, cancellationToken);
        if (membership is null) return TenancyErrors.MembershipNotFound;

        if (!Enum.TryParse<MembershipStatus>(request.Status, true, out var status))
            return TenancyErrors.MembershipInactive;

        var result = status switch
        {
            MembershipStatus.Active => membership.Activate(),
            MembershipStatus.Suspended => membership.Suspend(),
            _ => TenancyErrors.MembershipInactive
        };

        return result.IsFailure ? result.Error : Unit.Value;
    }
}
