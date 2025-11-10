using Authentication.Presentation.Endpoints;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthenticationPresentation(this IServiceCollection services)
    {
        services.AddTransient<LoginEndpoint>();
        services.AddTransient<RegisterEndpoint>();
        services.AddTransient<LogoutEndpoint>();
        services.AddTransient<GetSessionEndpoint>();

        return services;
    }

    public static IEndpointRouteBuilder ConfigureAuthenticationPresentation(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder
            .MapGroup(AuthRoutes.Base)
            .WithTags(EndpointTag.Authentication);

        builder.ServiceProvider.GetRequiredService<LoginEndpoint>().MapEndpoint(group);
        builder.ServiceProvider.GetRequiredService<RegisterEndpoint>().MapEndpoint(group);
        builder.ServiceProvider.GetRequiredService<LogoutEndpoint>().MapEndpoint(group);
        builder.ServiceProvider.GetRequiredService<GetSessionEndpoint>().MapEndpoint(group);

        return builder;
    }
}
