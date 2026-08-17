using Application.Abstractions.Security;

namespace Application.UnitTests.Helpers;

public sealed class FakeCurrentUser : ICurrentUser
{
    public Guid? UserId { get; set; }

    public string? Email { get; set; }

    public IReadOnlyCollection<string> Permissions { get; set; } = [];

    public bool IsAuthenticated => UserId.HasValue;

    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
