using Domain.Tenancy;
using Domain.Tenancy.ValueObjects;
using Shouldly;

namespace Domain.UnitTests.Tenancy;

public sealed class OrganizationTests
{
    [Fact]
    public void CreateRoot_SetsPlatformTypeAndPath()
    {
        var id = Guid.Parse("11111111-1111-1111-1111-111111111101");

        var result = Organization.CreateRoot(id, "Ranna", OrganizationSlug.Create("ranna").Value);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Type.ShouldBe(OrganizationType.Platform);
        result.Value.Depth.ShouldBe(0);
        result.Value.ParentId.ShouldBeNull();
        result.Value.Path.ShouldBe($"/{id:N}/");
        result.Value.IsRoot.ShouldBeTrue();
    }

    [Fact]
    public void CreateChild_UnderPlatform_CreatesOperatorWithNestedPath()
    {
        var root = Organization.CreateRoot(Guid.Parse("11111111-1111-1111-1111-111111111101"), "Ranna", OrganizationSlug.Create("ranna").Value).Value;
        var childId = Guid.Parse("11111111-1111-1111-1111-111111111102");

        var result = Organization.CreateChild(childId, root, "AquaCare", OrganizationSlug.Create("aquacare").Value);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Type.ShouldBe(OrganizationType.Operator);
        result.Value.Depth.ShouldBe(1);
        result.Value.ParentId.ShouldBe(root.Id);
        result.Value.Path.ShouldBe($"{root.Path}{childId:N}/");
        root.IsAncestorOf(result.Value.Path).ShouldBeTrue();
    }

    [Fact]
    public void CreateChild_UnderOperator_CreatesSupplier()
    {
        var root = Organization.CreateRoot(Guid.NewGuid(), "Ranna", OrganizationSlug.Create("ranna").Value).Value;
        var operatorOrg = Organization.CreateChild(Guid.NewGuid(), root, "AquaCare", OrganizationSlug.Create("aquacare").Value).Value;

        var result = Organization.CreateChild(Guid.NewGuid(), operatorOrg, "Supplier A", OrganizationSlug.Create("supplier-a").Value);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Type.ShouldBe(OrganizationType.Supplier);
        result.Value.Depth.ShouldBe(2);
    }

    [Fact]
    public void CreateChild_UnderSupplier_ReturnsFailure()
    {
        var root = Organization.CreateRoot(Guid.NewGuid(), "Ranna", OrganizationSlug.Create("ranna").Value).Value;
        var operatorOrg = Organization.CreateChild(Guid.NewGuid(), root, "AquaCare", OrganizationSlug.Create("aquacare").Value).Value;
        var supplier = Organization.CreateChild(Guid.NewGuid(), operatorOrg, "Supplier A", OrganizationSlug.Create("supplier-a").Value).Value;

        var result = Organization.CreateChild(Guid.NewGuid(), supplier, "Nested", OrganizationSlug.Create("nested").Value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TenancyErrors.CannotNestUnderSupplier);
    }

    [Fact]
    public void CreateChild_WhenParentSuspended_ReturnsFailure()
    {
        var root = Organization.CreateRoot(Guid.NewGuid(), "Ranna", OrganizationSlug.Create("ranna").Value).Value;
        root.Suspend();

        var result = Organization.CreateChild(Guid.NewGuid(), root, "AquaCare", OrganizationSlug.Create("aquacare").Value);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TenancyErrors.ParentNotActive);
    }
}
