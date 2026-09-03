using SharedKernel.Primitives;

namespace Domain.Identity;

public sealed class LoginAttempt : Entity
{
    public const int MaxEmailLength = 256;
    public const int MaxIpAddressLength = 64;

    public Guid? UserId { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public ClientType? ClientType { get; private set; }
    public string Email { get; private set; } = null!;
    public string? IpAddress { get; private set; }
    public bool Success { get; private set; }
    public DateTimeOffset AttemptedAt { get; private set; }

    private LoginAttempt() { }

    private LoginAttempt(
        Guid id,
        Guid? userId,
        string email,
        string? ipAddress,
        bool success,
        DateTimeOffset attemptedAt,
        Guid? organizationId,
        ClientType? clientType) : base(id)
    {
        UserId = userId;
        Email = email;
        IpAddress = ipAddress;
        Success = success;
        AttemptedAt = attemptedAt;
        OrganizationId = organizationId;
        ClientType = clientType;
    }

    internal static LoginAttempt CreateSuccess(
        Guid userId,
        string email,
        string? ipAddress,
        DateTimeOffset attemptedAt,
        Guid? organizationId = null,
        ClientType? clientType = null) =>
        new(Guid.NewGuid(), userId, NormalizeEmail(email), NormalizeIp(ipAddress), true, attemptedAt, organizationId, clientType);

    internal static LoginAttempt CreateFailure(
        Guid? userId,
        string email,
        string? ipAddress,
        DateTimeOffset attemptedAt,
        Guid? organizationId = null,
        ClientType? clientType = null) =>
        new(Guid.NewGuid(), userId, NormalizeEmail(email), NormalizeIp(ipAddress), false, attemptedAt, organizationId, clientType);

    private static string NormalizeEmail(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return normalized.Length > MaxEmailLength ? normalized[..MaxEmailLength] : normalized;
    }

    private static string? NormalizeIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return null;
        var trimmed = ipAddress.Trim();
        return trimmed.Length > MaxIpAddressLength ? trimmed[..MaxIpAddressLength] : trimmed;
    }
}
