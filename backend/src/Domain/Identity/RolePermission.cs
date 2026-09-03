using SharedKernel.Primitives;

namespace Domain.Identity;

public sealed class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public PermissionScope Scope { get; private set; }
    public Permission Permission { get; private set; } = null!;

    private RolePermission() { }

    internal static RolePermission Create(Guid roleId, Permission permission, PermissionScope scope) => new()
    {
        RoleId = roleId,
        PermissionId = permission.Id,
        Scope = scope,
        Permission = permission
    };

    internal void ChangeScope(PermissionScope scope) => Scope = scope;
}
