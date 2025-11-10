using System.Reflection;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Application.Abstractions.Messaging.Decorators;
using Core.Application.Abstractions.Messaging.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<>),
                typeof(ICommandHandler<,>)
                ), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(CommandHandlerLoggingDecorator<,>));

        return services;
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(
                typeof(IQueryHandler<,>)
                ), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));

        return services;
    }
}
