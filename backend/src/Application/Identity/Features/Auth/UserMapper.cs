using Application.Identity.Features.Auth.Queries.GetCurrentUser;
using Domain.Identity;
using Riok.Mapperly.Abstractions;

namespace Application.Identity.Features.Auth;

[Mapper]
public static partial class UserMapper
{
    public static CurrentUserDto ToCurrentUserDto(User user)
    {
        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Code)
            .Distinct()
            .ToArray();
        return new CurrentUserDto(user.Id, user.Email.Value, user.FullName.DisplayName, permissions);
    }
}
