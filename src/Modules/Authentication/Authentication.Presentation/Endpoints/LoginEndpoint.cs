using Authentication.Application.Login;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Authentication.Presentation.Endpoints;

internal sealed class LoginEndpoint : IEndpoint
{
    public sealed record LoginRequest(string Username, string Password, bool IsPersistent = false);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(AuthRoutes.Login, async delegate (LoginRequest request,
            ICommandHandler<LoginCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            LoginCommand loginCommand = new(request.Username, request.Password, request.IsPersistent);
            Result loginResult = await commandHandler.HandleAsync(loginCommand, cancellationToken);

            if (loginResult.IsSuccess)
            {
                return Results.NoContent();
            }

            return ErrorResults.FromError(loginResult.Error, StatusCodes.Status401Unauthorized);
        })
        .WithName("Login")
        .WithSummary("Authenticates a user")
        .WithDescription("Authenticates a user with username and password. Sets a session cookie upon success.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
    }
}
