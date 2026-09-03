using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity;

public sealed class Permission : Entity, IAuditableEntity
{
    public const int MaxCodeLength = 128;
    public const int MaxDescriptionLength = 256;
    public const int MaxModuleLength = 64;

    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Module { get; private set; } = "identity";
    public PermissionScope MaxScope { get; private set; } = PermissionScope.Subtree;
    public bool IsPlatformOnly { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    private Permission() { }

    private Permission(
        Guid id,
        string code,
        string? description,
        string module,
        PermissionScope maxScope,
        bool isPlatformOnly) : base(id)
    {
        Code = code;
        Description = description;
        Module = module;
        MaxScope = maxScope;
        IsPlatformOnly = isPlatformOnly;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Permission> Create(
        Guid id,
        string? code,
        string? description = null,
        string? module = null,
        PermissionScope maxScope = PermissionScope.Subtree,
        bool isPlatformOnly = false)
    {
        if (string.IsNullOrWhiteSpace(code)) return IdentityErrors.PermissionCodeRequired;
        var trimmedCode = code.Trim().ToLowerInvariant();
        if (trimmedCode.Length > MaxCodeLength) return IdentityErrors.PermissionCodeTooLong;
        var trimmedDescription = description?.Trim();
        if (trimmedDescription?.Length > MaxDescriptionLength) return IdentityErrors.PermissionCodeTooLong;

        var trimmedModule = string.IsNullOrWhiteSpace(module)
            ? trimmedCode.Split('.')[0]
            : module.Trim().ToLowerInvariant();
        if (trimmedModule.Length > MaxModuleLength) trimmedModule = trimmedModule[..MaxModuleLength];

        return new Permission(id, trimmedCode, trimmedDescription, trimmedModule, maxScope, isPlatformOnly);
    }

    public void SyncCatalog(PermissionScope maxScope, bool isPlatformOnly)
    {
        Module = Code.Contains('.') ? Code.Split('.')[0] : Code;
        MaxScope = maxScope;
        IsPlatformOnly = isPlatformOnly;
    }
}
