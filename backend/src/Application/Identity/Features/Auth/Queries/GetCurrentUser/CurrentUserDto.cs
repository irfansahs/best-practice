namespace Application.Identity.Features.Auth.Queries.GetCurrentUser;

public sealed record CurrentUserDto(Guid Id, string Email, string FullName, IReadOnlyCollection<string> Permissions);
