using System.Reflection;
using Core.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddCommandHandlers(assembly);
        services.AddQueryHandlers(assembly);

        return services;
    }
}
