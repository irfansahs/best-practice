using Application.Identity.Features.Auth.Queries.GetCurrentUser;
using Domain.Identity;
using Riok.Mapperly.Abstractions;

namespace Application.Identity.Features.Auth;

[Mapper]
public static partial class UserMapper
{
    public static CurrentUserDto ToCurrentUserDto(
        User user,
        IReadOnlyDictionary<string, int> permissions,
        OrganizationSummaryDto? activeOrganization,
        IReadOnlyCollection<OrganizationSummaryDto> organizations,
        bool isImpersonating) =>
        new(user.Id, user.Email.Value, user.FullName.DisplayName, permissions, activeOrganization, organizations, isImpersonating);
}
