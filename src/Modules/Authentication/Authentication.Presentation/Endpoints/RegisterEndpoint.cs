using Authentication.Application.Register;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Authentication.Presentation.Endpoints;

internal sealed class RegisterEndpoint : IEndpoint
{
    public sealed record RegisterRequest(string Username, string Email, string Password);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(AuthRoutes.Register, async delegate (RegisterRequest request,
            ICommandHandler<RegisterCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            RegisterCommand registerCommand = new(request.Username, request.Email, request.Password);
            Result registrationResult = await commandHandler.HandleAsync(registerCommand, cancellationToken);

            if (registrationResult.IsSuccess)
            {
                return Results.Created($"/api/users/{request.Username}", request.Username);
            }

            return ErrorResults.FromError(registrationResult.Error, StatusCodes.Status400BadRequest);
        })
        .WithName("Register")
        .WithSummary("Registers a new user")
        .WithDescription("Creates a new user account with the provided credentials and automatically logs them in.")
        .Produces(StatusCodes.Status201Created)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
