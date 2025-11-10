using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Users.Presentation.Endpoints;

namespace Users.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersPresentation(this IServiceCollection services)
    {
        services.AddTransient<GetUserByUsernameEndpoint>();

        return services;
    }

    public static IEndpointRouteBuilder ConfigureUsersPresentation(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder
            .MapGroup(ApiRoutes.Users.Base)
            .WithTags(EndpointTag.Users);

        builder.ServiceProvider.GetRequiredService<GetUserByUsernameEndpoint>().MapEndpoint(group);

        return builder;
    }
}
