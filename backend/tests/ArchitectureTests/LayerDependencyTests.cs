using NetArchTest.Rules;
using Shouldly;

namespace ArchitectureTests;

public sealed class LayerDependencyTests
{
    [Fact]
    public void Domain_ShouldNotReferenceExternalLayers()
    {
        var result = Types.InAssembly(typeof(Domain.Catalog.Product).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Application", "Infrastructure", "Api")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Application_ShouldNotReferenceInfrastructure()
    {
        var result = Types.InAssembly(typeof(Application.Dispatching.CqrsRegistration).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Infrastructure")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Api_ShouldOnlyReferenceApplicationAndInfrastructureProjects()
    {
        var projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "src", "Api", "Api.csproj"));

        var projectContent = File.ReadAllText(projectPath);
        var referencedProjects = System.Text.RegularExpressions.Regex.Matches(projectContent, @"ProjectReference Include=""\.\.\\([^""]+)""")
            .Select(match => Path.GetFileNameWithoutExtension(match.Groups[1].Value.Replace('\\', '/')))
            .OrderBy(name => name)
            .ToArray();

        referencedProjects.ShouldBe(["Application", "Infrastructure"]);
    }
}
