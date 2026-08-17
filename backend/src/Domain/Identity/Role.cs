using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity;

public sealed class Role : Entity, IAuditableEntity
{
    public const int MaxNameLength = 64;
    public const int MaxDescriptionLength = 256;

    private readonly List<Permission> _permissions = [];

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    private Role(Guid id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Role> Create(Guid id, string? name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return IdentityErrors.RoleNameRequired;
        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength) return IdentityErrors.RoleNameTooLong;
        var trimmedDescription = description?.Trim();
        if (trimmedDescription?.Length > MaxDescriptionLength) return IdentityErrors.RoleNameTooLong;
        return new Role(id, trimmedName, trimmedDescription);
    }

    public Result GrantPermission(Permission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id)) return Result.Success();
        _permissions.Add(permission);
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result RevokePermission(Guid permissionId)
    {
        var permission = _permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission is null) return IdentityErrors.PermissionNotFound;
        _permissions.Remove(permission);
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
}
