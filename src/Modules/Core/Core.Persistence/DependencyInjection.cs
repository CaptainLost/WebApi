using Core.Persistence.DomainEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddCorePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<DomainEventDispatcher>();
        services.AddSingleton<DomainEventDispatcherInterceptor>();

        return services;
    }
}
