using Application.Abstractions.Messaging.Queries;
using Application.Users.GetUserByUsername;
using Domain.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Common;

namespace Presentation.Endpoints.Users;

internal sealed class GetUserByUsernameEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(ApiRoutes.Users.GetByUsername, async delegate (string username,
            IQueryHandler<GetUserByUsernameQuery, UserResponse> queryHandler,
            CancellationToken cancellationToken)
        {
            GetUserByUsernameQuery query = new GetUserByUsernameQuery(username);
            Result<UserResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ErrorResults.FromError(result.Error);
        })
        .RequireAuthorization()
        .WithName("GetUserByUsername")
        .WithSummary("Gets a user by username")
        .WithDescription("Retrieves user information by their username. Requires authentication.")
        .WithTags(EndpointTag.Users)
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
