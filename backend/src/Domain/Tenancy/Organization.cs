using Domain.Identity.ValueObjects;
using Domain.Tenancy.Events;
using Domain.Tenancy.ValueObjects;
using SharedKernel.Abstractions;
using SharedKernel.Auditing;
using SharedKernel.Primitives;
using SharedKernel.Results;

namespace Domain.Tenancy;

public sealed class Organization : AggregateRoot, IAggregateRoot, IAuditableEntity, ISoftDeletable
{
    public const int MaxNameLength = 200;
    public const int MaxTimeZoneLength = 64;
    public const int MaxCultureLength = 16;
    public const int MaxPathLength = 450;

    public Guid? ParentId { get; private set; }
    public string Path { get; private set; } = null!;
    public int Depth { get; private set; }
    public OrganizationType Type { get; private set; }
    public string Name { get; private set; } = null!;
    public OrganizationSlug Slug { get; private set; } = null!;
    public OrganizationStatus Status { get; private set; }
    public Email? ContactEmail { get; private set; }
    public string TimeZoneId { get; private set; } = "UTC";
    public string DefaultCulture { get; private set; } = "en";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public bool IsActive => Status == OrganizationStatus.Active && !IsDeleted;
    public bool IsRoot => ParentId is null;

    private Organization() { }

    private Organization(
        Guid id,
        Guid? parentId,
        string path,
        int depth,
        OrganizationType type,
        string name,
        OrganizationSlug slug,
        Email? contactEmail,
        string timeZoneId,
        string defaultCulture) : base(id)
    {
        ParentId = parentId;
        Path = path;
        Depth = depth;
        Type = type;
        Name = name;
        Slug = slug;
        Status = OrganizationStatus.Active;
        ContactEmail = contactEmail;
        TimeZoneId = timeZoneId;
        DefaultCulture = defaultCulture;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Result<Organization> CreateRoot(
        Guid id,
        string? name,
        OrganizationSlug slug,
        Email? contactEmail = null,
        string? timeZoneId = null,
        string? defaultCulture = null)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure) return nameResult.Error;

        var org = new Organization(
            id,
            null,
            BuildPath(null, id),
            0,
            OrganizationType.Platform,
            nameResult.Value,
            slug,
            contactEmail,
            NormalizeTimeZone(timeZoneId),
            NormalizeCulture(defaultCulture));

        org.RaiseDomainEvent(new OrganizationCreatedEvent(org.Id, null, org.Type, org.Path));
        return org;
    }

    public static Result<Organization> CreateChild(
        Guid id,
        Organization parent,
        string? name,
        OrganizationSlug slug,
        Email? contactEmail = null,
        string? timeZoneId = null,
        string? defaultCulture = null)
    {
        if (!parent.IsActive) return TenancyErrors.ParentNotActive;
        if (parent.Type == OrganizationType.Supplier) return TenancyErrors.CannotNestUnderSupplier;

        var nameResult = ValidateName(name);
        if (nameResult.IsFailure) return nameResult.Error;

        var childType = parent.Type == OrganizationType.Platform
            ? OrganizationType.Operator
            : OrganizationType.Supplier;

        var org = new Organization(
            id,
            parent.Id,
            BuildPath(parent.Path, id),
            parent.Depth + 1,
            childType,
            nameResult.Value,
            slug,
            contactEmail,
            NormalizeTimeZone(timeZoneId),
            NormalizeCulture(defaultCulture));

        org.RaiseDomainEvent(new OrganizationCreatedEvent(org.Id, parent.Id, org.Type, org.Path));
        return org;
    }

    public Result Rename(string? name)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure) return nameResult.Error;
        Name = nameResult.Value;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result ChangeSlug(OrganizationSlug slug)
    {
        Slug = slug;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result UpdateDetails(Email? contactEmail, string? timeZoneId, string? defaultCulture)
    {
        ContactEmail = contactEmail;
        TimeZoneId = NormalizeTimeZone(timeZoneId);
        DefaultCulture = NormalizeCulture(defaultCulture);
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status == OrganizationStatus.Suspended) return Result.Success();
        Status = OrganizationStatus.Suspended;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new OrganizationStatusChangedEvent(Id, Status));
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == OrganizationStatus.Active) return Result.Success();
        Status = OrganizationStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new OrganizationStatusChangedEvent(Id, Status));
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == OrganizationStatus.Archived) return Result.Success();
        Status = OrganizationStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new OrganizationStatusChangedEvent(Id, Status));
        return Result.Success();
    }

    public bool IsAncestorOf(string descendantPath) =>
        !string.IsNullOrEmpty(descendantPath) && descendantPath.StartsWith(Path, StringComparison.Ordinal);

    public bool IsSameOrAncestorOf(Organization other) =>
        other.Id == Id || IsAncestorOf(other.Path);

    public void SoftDelete(DateTimeOffset deletedAt, string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        Status = OrganizationStatus.Archived;
        UpdatedAt = deletedAt;
    }

    public static string BuildPath(string? parentPath, Guid id) =>
        string.IsNullOrEmpty(parentPath) ? $"/{id:N}/" : $"{parentPath}{id:N}/";

    private static Result<string> ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return TenancyErrors.NameRequired;
        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength) return TenancyErrors.NameTooLong;
        return trimmed;
    }

    private static string NormalizeTimeZone(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId)) return "UTC";
        var trimmed = timeZoneId.Trim();
        return trimmed.Length > MaxTimeZoneLength ? trimmed[..MaxTimeZoneLength] : trimmed;
    }

    private static string NormalizeCulture(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture)) return "en";
        var trimmed = culture.Trim().ToLowerInvariant();
        return trimmed.Length > MaxCultureLength ? trimmed[..MaxCultureLength] : trimmed;
    }
}
