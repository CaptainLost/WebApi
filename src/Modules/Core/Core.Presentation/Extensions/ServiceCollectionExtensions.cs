using System.Reflection;
using Core.Presentation.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointsFromAssembly(this IServiceCollection services,
        Assembly assembly)
    {
        IEnumerable<Type> endpointTypes = assembly
            .GetTypes()
            .Where(type => type.IsClass &&
                          !type.IsAbstract &&
                          typeof(IEndpoint).IsAssignableFrom(type));

        foreach (Type endpointType in endpointTypes)
        {
            services.AddTransient(endpointType);
        }

        return services;
    }
}
