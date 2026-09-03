using Application.Abstractions.Messaging;
using Application.Identity.Features.Auth.Queries.GetCurrentUser;

namespace Application.Identity.Features.Auth.Queries.GetMyOrganizations;

public sealed record GetMyOrganizationsQuery : IQuery<IReadOnlyList<OrganizationSummaryDto>>;
