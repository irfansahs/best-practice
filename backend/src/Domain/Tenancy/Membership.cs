using Domain.Abstractions;
using Domain.Identity;
using Domain.Tenancy.Events;
using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Tenancy;

public sealed class Membership : Entity, IAuditableEntity, ISoftDeletable, ITenantScoped
{
    public const int MaxTitleLength = 100;

    private readonly List<MembershipRole> _roles = [];
    private readonly List<PermissionOverride> _overrides = [];

    public Guid UserId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string OrganizationPath { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public MembershipStatus Status { get; private set; }
    public string? Title { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }
    public Guid? InvitedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<MembershipRole> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<PermissionOverride> Overrides => _overrides.AsReadOnly();
    public bool IsActive => Status == MembershipStatus.Active && !IsDeleted;

    private Membership() { }

    private Membership(
        Guid id,
        Guid userId,
        Organization organization,
        bool isPrimary,
        string? title,
        DateTimeOffset joinedAt,
        Guid? invitedByUserId) : base(id)
    {
        UserId = userId;
        OrganizationId = organization.Id;
        OrganizationPath = organization.Path;
        IsPrimary = isPrimary;
        Status = MembershipStatus.Active;
        Title = title;
        JoinedAt = joinedAt;
        InvitedByUserId = invitedByUserId;
        CreatedAt = joinedAt;
    }

    public static Result<Membership> Create(
        Guid id,
        Guid userId,
        Organization organization,
        bool isPrimary,
        DateTimeOffset joinedAt,
        string? title = null,
        Guid? invitedByUserId = null)
    {
        if (!organization.IsActive) return TenancyErrors.OrganizationInactive;
        var titleResult = ValidateTitle(title);
        if (titleResult.IsFailure) return titleResult.Error;

        var membership = new Membership(id, userId, organization, isPrimary, titleResult.Value, joinedAt, invitedByUserId);
        membership.RaiseDomainEvent(new MembershipChangedEvent(userId, organization.Id));
        return membership;
    }

    public Result AssignRole(Role role, DateTimeOffset assignedAt, Guid? assignedByUserId = null)
    {
        if (_roles.Any(r => r.RoleId == role.Id)) return TenancyErrors.RoleAlreadyAssigned;
        _roles.Add(MembershipRole.Create(Id, role, assignedAt, assignedByUserId));
        UpdatedAt = assignedAt;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
        return Result.Success();
    }

    public Result RemoveRole(Guid roleId)
    {
        var existing = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (existing is null) return TenancyErrors.RoleNotAssigned;
        _roles.Remove(existing);
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
        return Result.Success();
    }

    public Result SetPrimary()
    {
        IsPrimary = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result ClearPrimary()
    {
        IsPrimary = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status == MembershipStatus.Suspended) return Result.Success();
        Status = MembershipStatus.Suspended;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == MembershipStatus.Active) return Result.Success();
        Status = MembershipStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
        return Result.Success();
    }

    public Result ChangeTitle(string? title)
    {
        var titleResult = ValidateTitle(title);
        if (titleResult.IsFailure) return titleResult.Error;
        Title = titleResult.Value;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result GrantOverride(Permission permission, PermissionEffect effect, PermissionScope scope, string? reason, DateTimeOffset? expiresAt)
    {
        var existing = _overrides.FirstOrDefault(o => o.PermissionId == permission.Id);
        if (existing is not null)
        {
            existing.Update(effect, scope, reason, expiresAt);
        }
        else
        {
            _overrides.Add(PermissionOverride.Create(Guid.NewGuid(), Id, permission, effect, scope, reason, expiresAt));
        }

        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
        return Result.Success();
    }

    public Result RemoveOverride(Guid permissionId)
    {
        var existing = _overrides.FirstOrDefault(o => o.PermissionId == permissionId);
        if (existing is null) return IdentityErrors.PermissionNotFound;
        _overrides.Remove(existing);
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
        return Result.Success();
    }

    public PermissionSet ResolvePermissions(DateTimeOffset utcNow)
    {
        var roleGrants = _roles
            .SelectMany(mr => mr.Role.RolePermissions)
            .Select(rp => new PermissionGrant(rp.Permission.Code, rp.Scope));

        var overrides = _overrides.Select(o => new PermissionOverrideGrant(
            o.Permission.Code,
            o.Effect,
            o.Scope,
            o.ExpiresAt));

        return PermissionSet.From(roleGrants, overrides, utcNow);
    }

    public bool AllowsClient(ClientType clientType) =>
        _roles.Count == 0
        || _roles.Any(r => r.Role.AllowsClient(clientType));

    public void AssignTenant(Guid organizationId, string organizationPath)
    {
        if (OrganizationId != Guid.Empty) return;
        OrganizationId = organizationId;
        OrganizationPath = organizationPath;
    }

    public void SoftDelete(DateTimeOffset deletedAt, string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        Status = MembershipStatus.Suspended;
        UpdatedAt = deletedAt;
        RaiseDomainEvent(new MembershipChangedEvent(UserId, OrganizationId));
    }

    private static Result<string?> ValidateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title)) return (string?)null;
        var trimmed = title.Trim();
        if (trimmed.Length > MaxTitleLength) return TenancyErrors.TitleTooLong;
        return trimmed;
    }
}
