namespace Domain.Identity;

public sealed record PermissionGrant(string Code, PermissionScope Scope);

public sealed record PermissionOverrideGrant(
    string Code,
    PermissionEffect Effect,
    PermissionScope Scope,
    DateTimeOffset? ExpiresAt);

public sealed class PermissionSet
{
    private readonly Dictionary<string, PermissionScope> _grants;

    public static PermissionSet Empty { get; } = new(new Dictionary<string, PermissionScope>(StringComparer.OrdinalIgnoreCase));

    private PermissionSet(Dictionary<string, PermissionScope> grants) => _grants = grants;

    public IReadOnlyDictionary<string, PermissionScope> Grants => _grants;

    public static PermissionSet FromScopes(IReadOnlyDictionary<string, int> scopes)
    {
        var map = new Dictionary<string, PermissionScope>(StringComparer.OrdinalIgnoreCase);
        foreach (var (code, scope) in scopes)
        {
            if (string.IsNullOrWhiteSpace(code)) continue;
            if (scope < (int)PermissionScope.Own || scope > (int)PermissionScope.Global) continue;
            map[code] = (PermissionScope)scope;
        }

        return new PermissionSet(map);
    }

    public static PermissionSet From(
        IEnumerable<PermissionGrant> roleGrants,
        IEnumerable<PermissionOverrideGrant>? overrides = null,
        DateTimeOffset? utcNow = null)
    {
        var map = new Dictionary<string, PermissionScope>(StringComparer.OrdinalIgnoreCase);

        foreach (var grant in roleGrants)
        {
            if (string.IsNullOrWhiteSpace(grant.Code)) continue;
            if (!map.TryGetValue(grant.Code, out var existing) || grant.Scope > existing)
                map[grant.Code] = grant.Scope;
        }

        if (overrides is not null)
        {
            var now = utcNow ?? DateTimeOffset.UtcNow;
            foreach (var item in overrides)
            {
                if (string.IsNullOrWhiteSpace(item.Code)) continue;
                if (item.ExpiresAt.HasValue && item.ExpiresAt.Value <= now) continue;

                if (item.Effect == PermissionEffect.Deny)
                {
                    map.Remove(item.Code);
                    continue;
                }

                if (!map.TryGetValue(item.Code, out var existing) || item.Scope > existing)
                    map[item.Code] = item.Scope;
            }
        }

        return new PermissionSet(map);
    }

    public bool Allows(string permission, PermissionScope minScope = PermissionScope.Organization) =>
        _grants.TryGetValue(permission, out var scope) && scope >= minScope;

    public PermissionScope? ScopeOf(string permission) =>
        _grants.TryGetValue(permission, out var scope) ? scope : null;

    public IReadOnlyList<string> ToClaimValues() =>
        _grants.Select(g => PermissionClaimFormatter.Format(g.Key, g.Value)).ToArray();
}

public static class PermissionClaimFormatter
{
    public static string Format(string code, PermissionScope scope) => $"{code}:{(int)scope}";

    public static bool TryParse(string value, out string code, out PermissionScope scope)
    {
        code = string.Empty;
        scope = PermissionScope.Organization;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var separator = value.LastIndexOf(':');
        if (separator <= 0 || separator == value.Length - 1)
        {
            code = value.Trim().ToLowerInvariant();
            scope = PermissionScope.Organization;
            return true;
        }

        if (!int.TryParse(value[(separator + 1)..], out var scopeValue)
            || scopeValue < (int)PermissionScope.Own
            || scopeValue > (int)PermissionScope.Global)
        {
            return false;
        }

        code = value[..separator].Trim().ToLowerInvariant();
        scope = (PermissionScope)scopeValue;
        return !string.IsNullOrEmpty(code);
    }
}
