using SharedKernel.Results;

namespace Domain.Tenancy;

public static class TenancyErrors
{
    public static Error NameRequired => Error.Validation("Tenancy.Organization.NameRequired", string.Empty);
    public static Error NameTooLong => Error.Validation("Tenancy.Organization.NameTooLong", string.Empty);
    public static Error SlugRequired => Error.Validation("Tenancy.Organization.SlugRequired", string.Empty);
    public static Error SlugInvalid => Error.Validation("Tenancy.Organization.SlugInvalid", string.Empty);
    public static Error SlugTooLong => Error.Validation("Tenancy.Organization.SlugTooLong", string.Empty);
    public static Error SlugAlreadyExists => Error.Conflict("Tenancy.Organization.SlugAlreadyExists", string.Empty);
    public static Error OrganizationNotFound => Error.NotFound("Tenancy.Organization.NotFound", string.Empty);
    public static Error OrganizationInactive => Error.Forbidden("Tenancy.Organization.Inactive", string.Empty);
    public static Error OrganizationSuspended => Error.Forbidden("Tenancy.Organization.Suspended", string.Empty);
    public static Error ParentRequired => Error.Validation("Tenancy.Organization.ParentRequired", string.Empty);
    public static Error ParentNotActive => Error.Validation("Tenancy.Organization.ParentNotActive", string.Empty);
    public static Error CannotNestUnderSupplier => Error.Validation("Tenancy.Organization.CannotNestUnderSupplier", string.Empty);
    public static Error InvalidOrganizationType => Error.Validation("Tenancy.Organization.InvalidType", string.Empty);
    public static Error RootMustBePlatform => Error.Validation("Tenancy.Organization.RootMustBePlatform", string.Empty);
    public static Error SystemOrganizationProtected => Error.Forbidden("Tenancy.Organization.SystemProtected", string.Empty);

    public static Error MembershipNotFound => Error.NotFound("Tenancy.Membership.NotFound", string.Empty);
    public static Error MembershipAlreadyExists => Error.Conflict("Tenancy.Membership.AlreadyExists", string.Empty);
    public static Error MembershipInactive => Error.Forbidden("Tenancy.Membership.Inactive", string.Empty);
    public static Error NoMembership => Error.Forbidden("Tenancy.Membership.None", string.Empty);
    public static Error RoleAlreadyAssigned => Error.Conflict("Tenancy.Membership.RoleAlreadyAssigned", string.Empty);
    public static Error RoleNotAssigned => Error.NotFound("Tenancy.Membership.RoleNotAssigned", string.Empty);
    public static Error TitleTooLong => Error.Validation("Tenancy.Membership.TitleTooLong", string.Empty);

    public static Error RoleNotAssignableToOrganization => Error.Forbidden("Tenancy.Role.NotAssignable", string.Empty);
    public static Error ClientTypeNotAllowed => Error.Forbidden("Tenancy.Role.ClientTypeNotAllowed", string.Empty);
    public static Error ImpersonationForbidden => Error.Forbidden("Tenancy.Organization.ImpersonationForbidden", string.Empty);
    public static Error TenantContextRequired => Error.Unauthorized("Tenancy.Context.Required", string.Empty);
    public static Error SwitchRequiresRefresh => Error.Validation("Tenancy.Switch.RefreshRequired", string.Empty);
}
