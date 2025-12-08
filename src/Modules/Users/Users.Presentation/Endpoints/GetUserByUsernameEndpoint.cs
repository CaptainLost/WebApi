using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.GetUserById;
using Users.Domain.Users;
using Core.Presentation.Extensions;

namespace Users.Presentation.Endpoints;

internal sealed class GetUserByUsernameEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet(UsersRoutes.GetByUsername, async delegate (Guid userId,
            IQueryHandler<GetUserByIdQuery, GetUserByIdResponse> queryHandler,
            CancellationToken cancellationToken)
        {
            GetUserByIdQuery query = new GetUserByIdQuery(userId);
            Result<GetUserByIdResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ErrorResults.FromError(result.Error, StatusCodes.Status404NotFound);
        })
        .RequireAuthorization(Permission.GetUser)
        .WithName("GetUserByUsername")
        .WithSummary("Gets a user by username")
        .WithDescription("Retrieves user information by their username.")
        .Produces<GetUserByIdResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}