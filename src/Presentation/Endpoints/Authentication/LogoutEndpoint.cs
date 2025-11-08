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
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(ApiRoutes.Auth.Logout, async delegate (
            ICommandHandler<LogoutCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            LogoutCommand logoutCommand = new LogoutCommand();
            Result logoutResult = await commandHandler.HandleAsync(logoutCommand, cancellationToken);

            if (logoutResult.IsSuccess)
            {
                return Results.NoContent();
            }

            return ErrorResults.FromError(logoutResult.Error);
        })
        .WithName("Logout")
        .WithSummary("Logs out the current user")
        .WithDescription("Logs out the authenticated user by clearing the session cookie.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
