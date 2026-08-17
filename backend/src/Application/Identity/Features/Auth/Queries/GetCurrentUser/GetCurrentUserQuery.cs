using Application.Abstractions.Messaging;

namespace Application.Identity.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<CurrentUserDto>;
