using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity;

public sealed class Role : Entity, IAuditableEntity
{
    public const int MaxNameLength = 64;
    public const int MaxDescriptionLength = 256;

    private readonly List<RolePermission> _rolePermissions = [];

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public string? OrganizationPath { get; private set; }
    public bool IsSystemRole { get; private set; }
    public ClientTypes AllowedClients { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role() { }

    private Role(
        Guid id,
        string name,
        string? description,
        Guid? organizationId,
        string? organizationPath,
        bool isSystemRole,
        ClientTypes allowedClients) : base(id)
    {
        Name = name;
        Description = description;
        OrganizationId = organizationId;
        OrganizationPath = organizationPath;
        IsSystemRole = isSystemRole;
        AllowedClients = allowedClients;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Role> Create(
        Guid id,
        string? name,
        string? description = null,
        Guid? organizationId = null,
        string? organizationPath = null,
        bool isSystemRole = false,
        ClientTypes allowedClients = ClientTypes.All)
    {
        if (string.IsNullOrWhiteSpace(name)) return IdentityErrors.RoleNameRequired;
        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength) return IdentityErrors.RoleNameTooLong;
        var trimmedDescription = description?.Trim();
        if (trimmedDescription?.Length > MaxDescriptionLength) return IdentityErrors.RoleNameTooLong;
        if (allowedClients == ClientTypes.None) allowedClients = ClientTypes.All;

        return new Role(id, trimmedName, trimmedDescription, organizationId, organizationPath, isSystemRole, allowedClients);
    }

    public Result Rename(string? name, string? description)
    {
        if (IsSystemRole) return IdentityErrors.SystemRoleProtected;
        var created = Create(Id, name, description, OrganizationId, OrganizationPath, IsSystemRole, AllowedClients);
        if (created.IsFailure) return created.Error;
        Name = created.Value.Name;
        Description = created.Value.Description;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result GrantPermission(Permission permission, PermissionScope scope)
    {
        if (scope > permission.MaxScope) return IdentityErrors.PermissionScopeExceedsMax;
        if (permission.IsPlatformOnly && OrganizationId is not null) return IdentityErrors.PlatformPermissionOnly;

        var existing = _rolePermissions.FirstOrDefault(p => p.PermissionId == permission.Id);
        if (existing is not null)
        {
            existing.ChangeScope(scope);
            UpdatedAt = DateTimeOffset.UtcNow;
            RaiseDomainEvent(new Events.RolePermissionsChangedEvent(Id));
            return Result.Success();
        }

        _rolePermissions.Add(RolePermission.Create(Id, permission, scope));
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new Events.RolePermissionsChangedEvent(Id));
        return Result.Success();
    }

    public Result RevokePermission(Guid permissionId)
    {
        var permission = _rolePermissions.FirstOrDefault(p => p.PermissionId == permissionId);
        if (permission is null) return IdentityErrors.PermissionNotFound;
        _rolePermissions.Remove(permission);
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new Events.RolePermissionsChangedEvent(Id));
        return Result.Success();
    }

    public void EnsureSystem(string name, string? description, ClientTypes allowedClients)
    {
        Name = name;
        Description = description;
        IsSystemRole = true;
        AllowedClients = allowedClients == ClientTypes.None ? ClientTypes.All : allowedClients;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool AllowsClient(ClientType clientType) =>
        AllowedClients.HasFlag(clientType == ClientType.Mobile ? ClientTypes.Mobile : ClientTypes.Web);

    public bool CanBeAssignedTo(string organizationPath)
    {
        if (IsSystemRole && OrganizationId is null) return true;
        if (string.IsNullOrEmpty(OrganizationPath)) return true;
        return organizationPath.StartsWith(OrganizationPath, StringComparison.Ordinal);
    }
}
