using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Security;
using Domain.Identity;
using Domain.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Identity.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(IAppDbContext db, IPasswordHasher passwordHasher) : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure) return emailResult.Error;

        var nameResult = FullName.Create(request.FirstName, request.LastName);
        if (nameResult.IsFailure) return nameResult.Error;

        var exists = await db.Users.AsNoTracking().AnyAsync(u => u.Email == emailResult.Value, cancellationToken);
        if (exists) return IdentityErrors.EmailAlreadyExists;

        var hashResult = PasswordHash.Create(passwordHasher.Hash(request.Password));
        if (hashResult.IsFailure) return hashResult.Error;

        var userResult = User.Register(Guid.NewGuid(), emailResult.Value, hashResult.Value, nameResult.Value);
        if (userResult.IsFailure) return userResult.Error;

        db.Users.Add(userResult.Value);
        return new RegisterResponse(userResult.Value.Id, userResult.Value.Email.Value);
    }
}
