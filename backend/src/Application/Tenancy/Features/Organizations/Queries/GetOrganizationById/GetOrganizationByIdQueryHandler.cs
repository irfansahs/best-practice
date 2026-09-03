using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Organizations.Queries.GetOrganizationById;

public sealed class GetOrganizationByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetOrganizationByIdQuery, OrganizationDetailDto>
{
    public async Task<Result<OrganizationDetailDto>> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        var org = await db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (org is null) return TenancyErrors.OrganizationNotFound;

        return new OrganizationDetailDto(
            org.Id,
            org.ParentId,
            org.Name,
            org.Slug.Value,
            org.Type.ToString(),
            org.Status.ToString(),
            org.Path,
            org.Depth,
            org.ContactEmail?.Value,
            org.TimeZoneId,
            org.DefaultCulture);
    }
}
