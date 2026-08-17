namespace Application.Abstractions.Security;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string permission);
}
