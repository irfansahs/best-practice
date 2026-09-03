using SharedKernel.Results;

namespace Domain.Identity;

public static class IdentityErrors
{
    public static Error EmailRequired => Error.Validation("Identity.Email.Required", string.Empty);
    public static Error EmailInvalid => Error.Validation("Identity.Email.Invalid", string.Empty);
    public static Error EmailTooLong => Error.Validation("Identity.Email.TooLong", string.Empty);
    public static Error EmailAlreadyExists => Error.Conflict("Identity.Email.AlreadyExists", string.Empty);

    public static Error PasswordHashRequired => Error.Validation("Identity.PasswordHash.Required", string.Empty);
    public static Error PasswordHashInvalid => Error.Validation("Identity.PasswordHash.Invalid", string.Empty);

    public static Error FirstNameRequired => Error.Validation("Identity.FullName.FirstNameRequired", string.Empty);
    public static Error LastNameRequired => Error.Validation("Identity.FullName.LastNameRequired", string.Empty);
    public static Error NameTooLong => Error.Validation("Identity.FullName.TooLong", string.Empty);

    public static Error UserNotFound => Error.NotFound("Identity.User.NotFound", string.Empty);
    public static Error UserInactive => Error.Forbidden("Identity.User.Inactive", string.Empty);
    public static Error UserAlreadyLocked => Error.Conflict("Identity.User.AlreadyLocked", string.Empty);
    public static Error UserNotLocked => Error.Conflict("Identity.User.NotLocked", string.Empty);

    public static Error RoleNotFound => Error.NotFound("Identity.Role.NotFound", string.Empty);
    public static Error RoleNameRequired => Error.Validation("Identity.Role.NameRequired", string.Empty);
    public static Error RoleNameTooLong => Error.Validation("Identity.Role.NameTooLong", string.Empty);
    public static Error RoleAlreadyAssigned => Error.Conflict("Identity.Role.AlreadyAssigned", string.Empty);
    public static Error RoleNotAssigned => Error.NotFound("Identity.Role.NotAssigned", string.Empty);

    public static Error PermissionNotFound => Error.NotFound("Identity.Permission.NotFound", string.Empty);
    public static Error PermissionCodeRequired => Error.Validation("Identity.Permission.CodeRequired", string.Empty);
    public static Error PermissionCodeTooLong => Error.Validation("Identity.Permission.CodeTooLong", string.Empty);

    public static Error RefreshTokenNotFound => Error.NotFound("Identity.RefreshToken.NotFound", string.Empty);
    public static Error RefreshTokenExpired => Error.Unauthorized("Identity.RefreshToken.Expired", string.Empty);
    public static Error RefreshTokenRevoked => Error.Unauthorized("Identity.RefreshToken.Revoked", string.Empty);
    public static Error RefreshTokenReuseDetected => Error.Unauthorized("Identity.RefreshToken.ReuseDetected", string.Empty);

    public static Error InvalidCredentials => Error.Unauthorized("Identity.Login.InvalidCredentials", string.Empty);
    public static Error ClientTypeRequired => Error.Validation("Identity.Login.ClientTypeRequired", string.Empty);
    public static Error ClientTypeNotAllowed => Error.Forbidden("Identity.Login.ClientTypeNotAllowed", string.Empty);
    public static Error SecurityStampMismatch => Error.Unauthorized("Identity.Token.SecurityStampMismatch", string.Empty);
    public static Error PermissionScopeExceedsMax => Error.Validation("Identity.Permission.ScopeExceedsMax", string.Empty);
    public static Error PlatformPermissionOnly => Error.Forbidden("Identity.Permission.PlatformOnly", string.Empty);
    public static Error SystemRoleProtected => Error.Forbidden("Identity.Role.SystemProtected", string.Empty);
}
