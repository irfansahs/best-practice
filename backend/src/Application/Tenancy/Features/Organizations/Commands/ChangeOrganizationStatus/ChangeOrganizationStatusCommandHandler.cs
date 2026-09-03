using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Organizations.Commands.ChangeOrganizationStatus;

public sealed class ChangeOrganizationStatusCommandHandler(IAppDbContext db) : IRequestHandler<ChangeOrganizationStatusCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ChangeOrganizationStatusCommand request, CancellationToken cancellationToken)
    {
        var org = await db.Organizations.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (org is null) return TenancyErrors.OrganizationNotFound;

        if (!Enum.TryParse<OrganizationStatus>(request.Status, true, out var status))
            return TenancyErrors.InvalidOrganizationType;

        var result = status switch
        {
            OrganizationStatus.Active => org.Activate(),
            OrganizationStatus.Suspended => org.Suspend(),
            OrganizationStatus.Archived => org.Archive(),
            _ => TenancyErrors.InvalidOrganizationType
        };

        return result.IsFailure ? result.Error : Unit.Value;
    }
}
