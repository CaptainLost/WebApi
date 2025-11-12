using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Enums;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.GetUserByUsername;

namespace Users.Presentation.Endpoints;

internal sealed class GetUserByUsernameEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet(UsersRoutes.GetByUsername, async delegate (string username,
            IQueryHandler<GetUserByUsernameQuery, GetUserResponse> queryHandler,
            CancellationToken cancellationToken)
        {
            GetUserByUsernameQuery query = new GetUserByUsernameQuery(username);
            Result<GetUserResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ErrorResults.FromError(result.Error, StatusCodes.Status404NotFound);
        })
        .RequireAuthorization(nameof(PermissionType.ReadUser))
        .WithName("GetUserByUsername")
        .WithSummary("Gets a user by username")
        .WithDescription("Retrieves user information by their username.")
        .Produces<GetUserResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}