using Domain.Identity;
using Shouldly;

namespace Domain.UnitTests.Identity;

public sealed class PermissionSetTests
{
    [Fact]
    public void From_SamePermission_KeepsWidestScope()
    {
        var set = PermissionSet.From(
        [
            new PermissionGrant("catalog.products.read", PermissionScope.Own),
            new PermissionGrant("catalog.products.read", PermissionScope.Subtree)
        ]);

        set.ScopeOf("catalog.products.read").ShouldBe(PermissionScope.Subtree);
        set.Allows("catalog.products.read", PermissionScope.Organization).ShouldBeTrue();
        set.Allows("catalog.products.read", PermissionScope.Global).ShouldBeFalse();
    }

    [Fact]
    public void From_DenyOverride_RemovesPermission()
    {
        var set = PermissionSet.From(
            [new PermissionGrant("catalog.products.delete", PermissionScope.Subtree)],
            [new PermissionOverrideGrant("catalog.products.delete", PermissionEffect.Deny, PermissionScope.Own, null)]);

        set.ScopeOf("catalog.products.delete").ShouldBeNull();
        set.Allows("catalog.products.delete").ShouldBeFalse();
    }

    [Fact]
    public void From_AllowOverride_WidensScope()
    {
        var set = PermissionSet.From(
            [new PermissionGrant("catalog.products.read", PermissionScope.Own)],
            [new PermissionOverrideGrant("catalog.products.read", PermissionEffect.Allow, PermissionScope.Organization, null)]);

        set.ScopeOf("catalog.products.read").ShouldBe(PermissionScope.Organization);
    }

    [Fact]
    public void From_ExpiredDeny_IsIgnored()
    {
        var now = DateTimeOffset.UtcNow;
        var set = PermissionSet.From(
            [new PermissionGrant("catalog.products.read", PermissionScope.Subtree)],
            [new PermissionOverrideGrant("catalog.products.read", PermissionEffect.Deny, PermissionScope.Own, now.AddMinutes(-1))],
            now);

        set.Allows("catalog.products.read", PermissionScope.Subtree).ShouldBeTrue();
    }

    [Fact]
    public void ClaimFormatter_RoundTripsCodeAndScope()
    {
        var value = PermissionClaimFormatter.Format("catalog.products.read", PermissionScope.Subtree);

        PermissionClaimFormatter.TryParse(value, out var code, out var scope).ShouldBeTrue();
        code.ShouldBe("catalog.products.read");
        scope.ShouldBe(PermissionScope.Subtree);
    }
}
