using SharedKernel.Primitives;

namespace Domain.Identity;

public sealed class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired(DateTimeOffset utcNow) => utcNow >= ExpiresAt;
    public bool IsActive(DateTimeOffset utcNow) => !IsRevoked && !IsExpired(utcNow);

    private RefreshToken() { }

    private RefreshToken(Guid id, Guid userId, string tokenHash, DateTimeOffset expiresAt, DateTimeOffset createdAt) : base(id)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    internal static RefreshToken Create(Guid id, Guid userId, string tokenHash, DateTimeOffset expiresAt, DateTimeOffset createdAt) =>
        new(id, userId, tokenHash, expiresAt, createdAt);

    public void Revoke(DateTimeOffset revokedAt, Guid? replacedByTokenId = null)
    {
        if (IsRevoked) return;
        RevokedAt = revokedAt;
        ReplacedByTokenId = replacedByTokenId;
    }
}
