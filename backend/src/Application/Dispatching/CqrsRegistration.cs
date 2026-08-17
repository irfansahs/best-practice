using System.Reflection;
using Application.Abstractions.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Dispatching;

public static class CqrsRegistration
{
    public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<HandlerCache>();
        services.AddScoped<IDispatcher, Dispatcher>();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(Abstractions.Events.IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.CachingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.TransactionBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.PerformanceBehavior<,>));

        return services;
    }
}
