using Domain.Identity;
using SharedKernel.Primitives;

namespace Domain.Tenancy;

public sealed class PermissionOverride : Entity
{
    public const int MaxReasonLength = 256;

    public Guid MembershipId { get; private set; }
    public Guid PermissionId { get; private set; }
    public PermissionEffect Effect { get; private set; }
    public PermissionScope Scope { get; private set; }
    public string? Reason { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public Permission Permission { get; private set; } = null!;

    private PermissionOverride() { }

    private PermissionOverride(Guid id) : base(id) { }

    internal static PermissionOverride Create(
        Guid id,
        Guid membershipId,
        Permission permission,
        PermissionEffect effect,
        PermissionScope scope,
        string? reason,
        DateTimeOffset? expiresAt) => new(id)
    {
        MembershipId = membershipId,
        PermissionId = permission.Id,
        Effect = effect,
        Scope = scope,
        Reason = NormalizeReason(reason),
        ExpiresAt = expiresAt,
        Permission = permission
    };

    internal void Update(PermissionEffect effect, PermissionScope scope, string? reason, DateTimeOffset? expiresAt)
    {
        Effect = effect;
        Scope = scope;
        Reason = NormalizeReason(reason);
        ExpiresAt = expiresAt;
    }

    private static string? NormalizeReason(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return null;
        var trimmed = reason.Trim();
        return trimmed.Length > MaxReasonLength ? trimmed[..MaxReasonLength] : trimmed;
    }
}
