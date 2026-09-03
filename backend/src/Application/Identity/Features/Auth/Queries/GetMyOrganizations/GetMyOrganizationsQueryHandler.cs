using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Application.Identity.Features.Auth.Queries.GetCurrentUser;
using Domain.Identity;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Queries.GetMyOrganizations;

public sealed class GetMyOrganizationsQueryHandler(IAppDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyOrganizationsQuery, IReadOnlyList<OrganizationSummaryDto>>
{
    public async Task<Result<IReadOnlyList<OrganizationSummaryDto>>> Handle(
        GetMyOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return IdentityErrors.UserNotFound;
        var organizations = await OrganizationDirectory.ListForUserAsync(
            db,
            currentUser.UserId.Value,
            currentUser,
            cancellationToken);
        return Result<IReadOnlyList<OrganizationSummaryDto>>.Success(organizations);
    }
}
