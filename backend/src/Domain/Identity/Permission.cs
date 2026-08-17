using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity;

public sealed class Permission : Entity, IAuditableEntity
{
    public const int MaxCodeLength = 128;
    public const int MaxDescriptionLength = 256;

    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    private Permission() { }

    private Permission(Guid id, string code, string? description) : base(id)
    {
        Code = code;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Permission> Create(Guid id, string? code, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code)) return IdentityErrors.PermissionCodeRequired;
        var trimmedCode = code.Trim().ToLowerInvariant();
        if (trimmedCode.Length > MaxCodeLength) return IdentityErrors.PermissionCodeTooLong;
        var trimmedDescription = description?.Trim();
        if (trimmedDescription?.Length > MaxDescriptionLength) return IdentityErrors.PermissionCodeTooLong;
        return new Permission(id, trimmedCode, trimmedDescription);
    }
}
