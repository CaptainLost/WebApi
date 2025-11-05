using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Endpoints;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IEndpoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>(), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        IEnumerable<IEndpoint> endpoints = routeBuilder.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(routeBuilder);
        }

        return routeBuilder;
    }
}