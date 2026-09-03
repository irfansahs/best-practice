using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Tenancy;
using Domain.Identity.ValueObjects;
using Domain.Tenancy;
using Domain.Tenancy.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Organizations.Commands.CreateOrganization;

public sealed class CreateOrganizationCommandHandler(IAppDbContext db, ITenantContext tenantContext)
    : IRequestHandler<CreateOrganizationCommand, CreateOrganizationResponse>
{
    public async Task<Result<CreateOrganizationResponse>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsAvailable) return TenancyErrors.TenantContextRequired;

        var slugResult = OrganizationSlug.Create(request.Slug);
        if (slugResult.IsFailure) return slugResult.Error;

        var slugExists = await db.Organizations.IgnoreQueryFilters()
            .AnyAsync(o => o.Slug == slugResult.Value && !o.IsDeleted, cancellationToken);
        if (slugExists) return TenancyErrors.SlugAlreadyExists;

        Email? email = null;
        if (!string.IsNullOrWhiteSpace(request.ContactEmail))
        {
            var emailResult = Email.Create(request.ContactEmail);
            if (emailResult.IsFailure) return emailResult.Error;
            email = emailResult.Value;
        }

        var parentId = request.ParentId ?? tenantContext.OrganizationId;
        var parent = await db.Organizations.FirstOrDefaultAsync(o => o.Id == parentId, cancellationToken);
        if (parent is null) return TenancyErrors.OrganizationNotFound;

        var created = Organization.CreateChild(Guid.NewGuid(), parent, request.Name, slugResult.Value, email, request.TimeZoneId, request.DefaultCulture);
        if (created.IsFailure) return created.Error;

        db.Organizations.Add(created.Value);
        return new CreateOrganizationResponse(created.Value.Id, created.Value.Slug.Value);
    }
}
