using System.Reflection;
using Authentication.Presentation.Endpoints;
using Core.Presentation.Endpoints;
using Core.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationPresentation(this IServiceCollection services)
    {
        services.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IEndpointRouteBuilder ConfigureAuthenticationPresentation(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder
            .MapGroup(AuthRoutes.Base)
            .WithTags(EndpointTag.Authentication);

        builder.MapEndpointsFromAssembly(Assembly.GetExecutingAssembly(), group);

        return builder;
    }
}
