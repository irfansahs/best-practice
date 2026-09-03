using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Identity.ValueObjects;
using Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Tenancy.Features.Organizations.Commands.UpdateOrganization;

public sealed class UpdateOrganizationCommandHandler(IAppDbContext db) : IRequestHandler<UpdateOrganizationCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var org = await db.Organizations.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (org is null) return TenancyErrors.OrganizationNotFound;

        var rename = org.Rename(request.Name);
        if (rename.IsFailure) return rename.Error;

        Email? email = null;
        if (!string.IsNullOrWhiteSpace(request.ContactEmail))
        {
            var emailResult = Email.Create(request.ContactEmail);
            if (emailResult.IsFailure) return emailResult.Error;
            email = emailResult.Value;
        }

        org.UpdateDetails(email, request.TimeZoneId, request.DefaultCulture);
        return Unit.Value;
    }
}
