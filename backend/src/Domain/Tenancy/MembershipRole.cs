using Domain.Identity;
using SharedKernel.Primitives;

namespace Domain.Tenancy;

public sealed class MembershipRole
{
    public Guid MembershipId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }
    public Guid? AssignedByUserId { get; private set; }
    public Role Role { get; private set; } = null!;

    private MembershipRole() { }

    internal static MembershipRole Create(Guid membershipId, Role role, DateTimeOffset assignedAt, Guid? assignedByUserId) => new()
    {
        MembershipId = membershipId,
        RoleId = role.Id,
        AssignedAt = assignedAt,
        AssignedByUserId = assignedByUserId,
        Role = role
    };
}
