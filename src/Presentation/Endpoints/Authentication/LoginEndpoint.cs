using Application.Abstractions.Messaging.Commands;
using Application.Authentication.Login;
using Domain.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Common;

namespace Presentation.Endpoints.Authentication;

internal sealed class LoginEndpoint : IEndpoint
{
    public sealed record LoginRequest(string Username, string Password);

    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost(ApiRoutes.Auth.Login, async delegate (LoginRequest request,
            ICommandHandler<LoginCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            LoginCommand loginCommand = new LoginCommand(request.Username, request.Password);
            Result loginResult = await commandHandler.HandleAwait(loginCommand, cancellationToken);

            if (loginResult.IsSuccess)
            {
                return Results.NoContent();
            }

            return ErrorResults.FromError(loginResult.Error);
        })
        .WithName("Login")
        .WithSummary("Authenticates a user")
        .WithDescription("Authenticates a user with username and password. Sets a session cookie upon success.")
        .WithTags(EndpointTag.Authentication)
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
    }
}
