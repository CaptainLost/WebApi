using Application.Abstractions.Messaging.Commands;
using Application.Abstractions.Messaging.Decorators;
using Application.Abstractions.Messaging.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();

        services.Scan(scan => scan
            .FromAssemblies(executingAssembly)
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

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();

        services.Scan(scan => scan
            .FromAssemblies(executingAssembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));

        return services;
    }
}
