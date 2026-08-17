using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(IAppDbContext db, ICurrentUser currentUser, IPasswordHasher passwordHasher) : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null) return IdentityErrors.UserNotFound;

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == currentUser.UserId, cancellationToken);
        if (user is null) return IdentityErrors.UserNotFound;

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash.Value))
            return IdentityErrors.InvalidCredentials;

        var hashResult = PasswordHash.Create(passwordHasher.Hash(request.NewPassword));
        if (hashResult.IsFailure) return hashResult.Error;

        var changeResult = user.ChangePassword(hashResult.Value);
        if (changeResult.IsFailure) return changeResult.Error;

        return Unit.Value;
    }
}
