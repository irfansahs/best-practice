using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Organizations.Queries.GetOrganizations;

public sealed class GetOrganizationsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetOrganizationsQuery, IReadOnlyList<OrganizationListItemDto>>
{
    public async Task<Result<IReadOnlyList<OrganizationListItemDto>>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.Organizations.AsNoTracking()
            .OrderBy(o => o.Path)
            .Select(o => new OrganizationListItemDto(o.Id, o.ParentId, o.Name, o.Slug.Value, o.Type.ToString(), o.Status.ToString(), o.Path, o.Depth))
            .ToListAsync(cancellationToken);

        return items;
    }
}
