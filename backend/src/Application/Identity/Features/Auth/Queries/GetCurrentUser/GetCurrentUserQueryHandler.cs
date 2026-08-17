using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IAppDbContext db, ICurrentUser currentUser) : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return IdentityErrors.UserNotFound;

        var user = await db.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);

        if (user is null) return IdentityErrors.UserNotFound;
        return UserMapper.ToCurrentUserDto(user);
    }
}
