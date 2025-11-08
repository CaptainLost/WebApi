using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Endpoints;
using Presentation.Endpoints.Authentication;
using Presentation.Endpoints.Users;

namespace Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IEndpoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>(), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapAuthenticationEndpoints();
        routeBuilder.MapUsersEndpoints();

        return routeBuilder;
    }
}