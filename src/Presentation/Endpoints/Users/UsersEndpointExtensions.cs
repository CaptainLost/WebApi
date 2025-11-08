using Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Endpoints.Users;

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
