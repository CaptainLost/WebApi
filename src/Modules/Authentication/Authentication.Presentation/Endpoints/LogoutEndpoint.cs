using Authentication.Application.Logout;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Authentication.Presentation.Endpoints;

internal sealed class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(AuthRoutes.Logout, async delegate (
            ICommandHandler<LogoutCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            LogoutCommand logoutCommand = new();
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
