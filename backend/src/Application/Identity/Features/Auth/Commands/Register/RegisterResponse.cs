namespace Application.Identity.Features.Auth.Commands.Register;

public sealed record RegisterResponse(Guid UserId, string Email);
