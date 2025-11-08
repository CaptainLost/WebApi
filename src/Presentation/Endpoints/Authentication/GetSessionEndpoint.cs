using Application.Abstractions.Messaging.Queries;
using Application.Authentication.GetSession;
using Domain.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Common;

namespace Presentation.Endpoints.Authentication;

internal sealed class GetSessionEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet(ApiRoutes.Auth.Session, async delegate (
            IQueryHandler<GetSessionQuery, SessionResponse> queryHandler,
            CancellationToken cancellationToken)
        {
            GetSessionQuery query = new GetSessionQuery();
            Result<SessionResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ErrorResults.FromError(result.Error);
        })
        .WithName("GetSession")
        .WithSummary("Get current session information")
        .WithDescription("Returns information about the current authenticated user session.")
        .Produces<SessionResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
