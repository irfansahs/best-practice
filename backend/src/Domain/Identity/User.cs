using Domain.Identity.Events;
using Domain.Identity.ValueObjects;
using SharedKernel.Auditing;
using SharedKernel.Abstractions;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Identity;

public sealed class User : AggregateRoot, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    private readonly List<Role> _roles = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    private readonly List<LoginAttempt> _loginAttempts = [];

    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public FullName FullName { get; private set; } = null!;
    public Guid SecurityStamp { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsLockedOut { get; private set; }
    public DateTimeOffset? LockoutEnd { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public IReadOnlyCollection<LoginAttempt> LoginAttempts => _loginAttempts.AsReadOnly();

    private User() { }

    private User(Guid id, Email email, PasswordHash passwordHash, FullName fullName) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        SecurityStamp = Guid.NewGuid();
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<User> Register(Guid id, Email email, PasswordHash passwordHash, FullName fullName)
    {
        var user = new User(id, email, passwordHash, fullName);
        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value));
        return user;
    }

    public Result ChangePassword(PasswordHash newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        RotateSecurityStamp();
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive) return Result.Success();
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive) return Result.Success();
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result RecordSuccessfulLogin(string? ipAddress, DateTimeOffset attemptedAt, Guid? organizationId = null, ClientType? clientType = null)
    {
        FailedLoginAttempts = 0;
        IsLockedOut = false;
        LockoutEnd = null;
        LastLoginAt = attemptedAt;
        _loginAttempts.Add(LoginAttempt.CreateSuccess(Id, Email.Value, ipAddress, attemptedAt, organizationId, clientType));
        UpdatedAt = attemptedAt;
        return Result.Success();
    }

    public Result RecordFailedLogin(
        string? ipAddress,
        DateTimeOffset attemptedAt,
        int maxFailedAttempts,
        TimeSpan lockoutDuration,
        Guid? organizationId = null,
        ClientType? clientType = null)
    {
        FailedLoginAttempts++;
        _loginAttempts.Add(LoginAttempt.CreateFailure(Id, Email.Value, ipAddress, attemptedAt, organizationId, clientType));

        if (FailedLoginAttempts < maxFailedAttempts)
        {
            UpdatedAt = attemptedAt;
            return Result.Success();
        }

        IsLockedOut = true;
        LockoutEnd = attemptedAt.Add(lockoutDuration);
        UpdatedAt = attemptedAt;
        RaiseDomainEvent(new UserLockedOutEvent(Id, Email.Value, LockoutEnd.Value));
        return Result.Success();
    }

    public bool IsLockoutActive(DateTimeOffset utcNow) =>
        IsLockedOut && LockoutEnd.HasValue && LockoutEnd.Value > utcNow;

    public Result Unlock()
    {
        if (!IsLockedOut) return IdentityErrors.UserNotLocked;
        IsLockedOut = false;
        LockoutEnd = null;
        FailedLoginAttempts = 0;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result AssignRole(Role role)
    {
        if (_roles.Any(r => r.Id == role.Id)) return IdentityErrors.RoleAlreadyAssigned;
        _roles.Add(role);
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result RemoveRole(Guid roleId)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role is null) return IdentityErrors.RoleNotAssigned;
        _roles.Remove(role);
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public RefreshToken IssueRefreshToken(
        Guid tokenId,
        string tokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt,
        Guid organizationId,
        Guid familyId,
        ClientType clientType,
        bool isImpersonating = false,
        string? deviceId = null,
        string? deviceName = null,
        string? createdByIp = null)
    {
        var token = RefreshToken.Create(
            tokenId,
            Id,
            tokenHash,
            expiresAt,
            createdAt,
            organizationId,
            familyId,
            clientType,
            isImpersonating,
            deviceId,
            deviceName,
            createdByIp);
        _refreshTokens.Add(token);
        UpdatedAt = createdAt;
        return token;
    }

    public Result RevokeRefreshToken(
        Guid tokenId,
        DateTimeOffset revokedAt,
        Guid? replacedByTokenId = null,
        RefreshTokenRevokeReason reason = RefreshTokenRevokeReason.Rotated)
    {
        var token = _refreshTokens.FirstOrDefault(t => t.Id == tokenId);
        if (token is null) return IdentityErrors.RefreshTokenNotFound;
        token.Revoke(revokedAt, replacedByTokenId, reason);
        UpdatedAt = revokedAt;
        return Result.Success();
    }

    public int RevokeRefreshTokenFamily(Guid familyId, DateTimeOffset revokedAt, RefreshTokenRevokeReason reason)
    {
        var count = 0;
        foreach (var token in _refreshTokens.Where(t => t.FamilyId == familyId && !t.IsRevoked))
        {
            token.Revoke(revokedAt, reason: reason);
            count++;
        }

        UpdatedAt = revokedAt;
        return count;
    }

    public int RevokeAllRefreshTokens(DateTimeOffset revokedAt, RefreshTokenRevokeReason reason)
    {
        var count = 0;
        foreach (var token in _refreshTokens.Where(t => !t.IsRevoked))
        {
            token.Revoke(revokedAt, reason: reason);
            count++;
        }

        UpdatedAt = revokedAt;
        return count;
    }

    public void RotateSecurityStamp()
    {
        SecurityStamp = Guid.NewGuid();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete(DateTimeOffset deletedAt, string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        IsActive = false;
        UpdatedAt = deletedAt;
        RotateSecurityStamp();
    }
}
