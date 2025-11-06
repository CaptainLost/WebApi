using Application.Abstractions.Messaging.Commands;
using Application.Authentication.Logout;
using Domain.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Common;

namespace Presentation.Endpoints.Authentication;

internal sealed class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("api/auth/logout", async delegate (
            ICommandHandler<LogoutCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            LogoutCommand logoutCommand = new LogoutCommand();
            Result logoutResult = await commandHandler.HandleAwait(logoutCommand, cancellationToken);

            if (logoutResult.IsSuccess)
            {
                return Results.NoContent();
            }

            return ErrorResults.FromError(logoutResult.Error);
        })
        .WithName("Logout")
        .WithSummary("Logs out the current user")
        .WithDescription("Logs out the authenticated user by clearing the session cookie.")
        .WithTags(EndpointTag.Authentication)
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
