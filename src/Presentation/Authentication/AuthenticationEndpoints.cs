using Application.Abstractions.Messaging.Commands;
using Application.Authentication.Login;
using Application.Authentication.Register;
using Domain.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Common;

namespace Presentation.Authentication;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder authGroup = app.MapGroup("api/auth")
            .WithTags("Authentication");

        authGroup.MapPost("login", async (LoginCommand command, ICommandDispatcher commandDispatcher) =>
        {
            Result loginResult = await commandDispatcher.Dispatch(command);
            
            if (loginResult.IsSuccess)
            {
                return Results.NoContent();
            }

            return ErrorResults.FromError(loginResult.Error);
        })
        .WithName("Login")
        .WithSummary("Authenticates a user")
        .WithDescription("Authenticates a user with username and password. Sets a session cookie upon success.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);

        authGroup.MapPost("register", async (RegisterCommand command, ICommandDispatcher commandDispatcher) =>
        {
            Result registrationResult = await commandDispatcher.Dispatch(command);
            
            if (registrationResult.IsSuccess)
            {
                return Results.Created($"/api/users/{command.Username}", command.Username);
            }

            return ErrorResults.FromError(registrationResult.Error);
        })
        .WithName("Register")
        .WithSummary("Registers a new user")
        .WithDescription("Creates a new user account with the provided credentials and automatically logs them in.")
        .Produces(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict);
    }
}
