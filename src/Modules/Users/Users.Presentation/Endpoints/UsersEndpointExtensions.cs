using Core.Domain.Enums;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Presentation.Endpoints;

internal static class UsersEndpointExtensions
{
    internal static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder
            .MapGroup(ApiRoutes.Users.Base)
            .RequireAuthorization(nameof(PermissionType.AccessUsers))
            .WithTags(EndpointTag.Users);

        builder.ServiceProvider.GetRequiredService<GetUserByUsernameEndpoint>().MapEndpoint(group);

        return builder;
    }
}
