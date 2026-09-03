using Application.Abstractions.Security;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Security;

public sealed class PermissionRequirement(string permission, PermissionScope minScope) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
    public PermissionScope MinScope { get; } = minScope;
}

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        foreach (var claim in context.User.FindAll(AuthClaims.Permission))
        {
            if (!PermissionClaimFormatter.TryParse(claim.Value, out var code, out var scope))
                continue;

            if (code.Equals(requirement.Permission, StringComparison.OrdinalIgnoreCase) && scope >= requirement.MinScope)
            {
                context.Succeed(requirement);
                break;
            }
        }

        return Task.CompletedTask;
    }
}
