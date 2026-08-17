using Application.Abstractions.Messaging;

namespace Application.Identity.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName) : ICommand<RegisterResponse>;
