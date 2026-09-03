using Domain.Abstractions;
using Domain.Catalog;
using Shouldly;

namespace ArchitectureTests;

public sealed class TenantScopeConventionTests
{
    [Fact]
    public void CatalogAggregates_ShouldImplementITenantScoped()
    {
        typeof(ITenantScoped).IsAssignableFrom(typeof(Product)).ShouldBeTrue();
        typeof(ITenantScoped).IsAssignableFrom(typeof(Category)).ShouldBeTrue();
    }

    [Fact]
    public void ApplicationTenancy_ShouldNotReferenceInfrastructure()
    {
        var result = NetArchTest.Rules.Types
            .InAssembly(typeof(Application.Dispatching.CqrsRegistration).Assembly)
            .That()
            .ResideInNamespace("Application.Tenancy")
            .ShouldNot()
            .HaveDependencyOn("Infrastructure")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }
}
