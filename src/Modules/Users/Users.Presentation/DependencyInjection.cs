using System.Reflection;
using Core.Presentation.Extensions;
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
        services.AddEndpointsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IEndpointRouteBuilder ConfigureUsersPresentation(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder
            .MapGroup(UsersRoutes.Base)
            .WithTags(EndpointTag.Users);

        builder.MapEndpointsFromAssembly(Assembly.GetExecutingAssembly(), group);

        return builder;
    }
}
