using Application.Abstractions.Messaging.Commands;
using Application.Authentication.Register;
using Domain.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Presentation.Common;

namespace Presentation.Endpoints.Authentication;

internal sealed class RegisterEndpoint : IEndpoint
{
    public sealed record RegisterRequest(string Username, string Email, string Password);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(ApiRoutes.Auth.Register, async delegate (RegisterRequest request,
            ICommandHandler<RegisterCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            RegisterCommand registerCommand = new RegisterCommand(request.Username, request.Email, request.Password);
            Result registrationResult = await commandHandler.HandleAsync(registerCommand, cancellationToken);

            if (registrationResult.IsSuccess)
            {
                return Results.Created($"/api/users/{request.Username}", request.Username);
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
