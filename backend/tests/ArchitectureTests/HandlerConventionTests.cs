using Application.Catalog.Features.Products.Commands.CreateProduct;
using Application.Catalog.Features.Products.Queries.GetProductById;
using Shouldly;

namespace ArchitectureTests;

public sealed class HandlerConventionTests
{
    [Fact]
    public void CommandAndQueryHandlers_ShouldLiveInMatchingFeatureFolders()
    {
        var assembly = typeof(CreateProductCommandHandler).Assembly;
        var handlers = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Handler", StringComparison.Ordinal))
            .Where(type => type.Namespace?.Contains(".Features.", StringComparison.Ordinal) == true)
            .Where(type => !type.Namespace!.Contains(".EventHandlers.", StringComparison.Ordinal))
            .ToArray();

        handlers.ShouldNotBeEmpty();

        foreach (var handler in handlers)
        {
            if (handler.Name.EndsWith("CommandHandler", StringComparison.Ordinal))
                handler.Namespace!.ShouldContain(".Commands.", Case.Insensitive);
            else if (handler.Name.EndsWith("QueryHandler", StringComparison.Ordinal))
                handler.Namespace!.ShouldContain(".Queries.", Case.Insensitive);
        }
    }

    [Fact]
    public void SampleHandlers_ShouldMatchSliceNamespaces()
    {
        typeof(CreateProductCommandHandler).Namespace!
            .ShouldBe("Application.Catalog.Features.Products.Commands.CreateProduct");
        typeof(GetProductByIdQueryHandler).Namespace!
            .ShouldBe("Application.Catalog.Features.Products.Queries.GetProductById");
    }
}
