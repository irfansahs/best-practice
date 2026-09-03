using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Security;

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    public const string PermissionPrefix = "Permission:";
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var remainder = policyName[PermissionPrefix.Length..];
            var (permission, minScope) = Parse(remainder);
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission, minScope))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

    private static (string Permission, PermissionScope MinScope) Parse(string remainder)
    {
        var separator = remainder.LastIndexOf(':');
        if (separator > 0
            && int.TryParse(remainder[(separator + 1)..], out var scopeValue)
            && scopeValue is >= (int)PermissionScope.Own and <= (int)PermissionScope.Global)
        {
            return (remainder[..separator], (PermissionScope)scopeValue);
        }

        return (remainder, PermissionScope.Organization);
    }
}
