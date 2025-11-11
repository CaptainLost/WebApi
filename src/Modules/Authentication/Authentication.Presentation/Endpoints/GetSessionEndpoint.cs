using Authentication.Application.GetSession;
using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Authentication.Presentation.Endpoints;

internal sealed class GetSessionEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet(AuthRoutes.Session, async delegate (
            IQueryHandler<GetSessionQuery, SessionResponse> queryHandler,
            CancellationToken cancellationToken)
        {
            GetSessionQuery query = new();
            Result<SessionResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ErrorResults.FromError(result.Error, StatusCodes.Status400BadRequest);
        })
        .WithName("GetSession")
        .WithSummary("Get current session information")
        .WithDescription("Returns information about the current authenticated user session.")
        .Produces<SessionResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
