using SharedKernel.Primitives;

namespace Domain.Identity;

public sealed class RefreshToken : Entity
{
    public const int MaxDeviceIdLength = 128;
    public const int MaxDeviceNameLength = 128;
    public const int MaxIpAddressLength = 64;

    public Guid UserId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid FamilyId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public RefreshTokenRevokeReason? RevokedReason { get; private set; }
    public ClientType ClientType { get; private set; }
    public bool IsImpersonating { get; private set; }
    public string? DeviceId { get; private set; }
    public string? DeviceName { get; private set; }
    public string? CreatedByIp { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired(DateTimeOffset utcNow) => utcNow >= ExpiresAt;
    public bool IsActive(DateTimeOffset utcNow) => !IsRevoked && !IsExpired(utcNow);

    private RefreshToken() { }

    private RefreshToken(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt,
        Guid organizationId,
        Guid familyId,
        ClientType clientType,
        bool isImpersonating,
        string? deviceId,
        string? deviceName,
        string? createdByIp) : base(id)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        OrganizationId = organizationId;
        FamilyId = familyId;
        ClientType = clientType;
        IsImpersonating = isImpersonating;
        DeviceId = Truncate(deviceId, MaxDeviceIdLength);
        DeviceName = Truncate(deviceName, MaxDeviceNameLength);
        CreatedByIp = Truncate(createdByIp, MaxIpAddressLength);
    }

    internal static RefreshToken Create(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt,
        Guid organizationId,
        Guid familyId,
        ClientType clientType,
        bool isImpersonating = false,
        string? deviceId = null,
        string? deviceName = null,
        string? createdByIp = null) =>
        new(id, userId, tokenHash, expiresAt, createdAt, organizationId, familyId, clientType, isImpersonating, deviceId, deviceName, createdByIp);

    public void Revoke(
        DateTimeOffset revokedAt,
        Guid? replacedByTokenId = null,
        RefreshTokenRevokeReason reason = RefreshTokenRevokeReason.Rotated)
    {
        if (IsRevoked) return;
        RevokedAt = revokedAt;
        ReplacedByTokenId = replacedByTokenId;
        RevokedReason = reason;
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        return trimmed.Length > maxLength ? trimmed[..maxLength] : trimmed;
    }
}
