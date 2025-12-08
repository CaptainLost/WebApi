using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.RegisterUser;

namespace Users.Presentation.Endpoints;

internal sealed class RegisterEndpoint : IEndpoint
{
    public sealed record RegisterRequest(string Username, string Email, string Password);
    public sealed record RegisterResponse(string Token);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.Register, async delegate (RegisterRequest request,
            ICommandHandler<RegisterUserCommand, string> commandHandler,
            CancellationToken cancellationToken)
        {
            RegisterUserCommand registerCommand = new(request.Username, request.Email, request.Password);
            Result<string> tokenResult = await commandHandler.HandleAsync(registerCommand, cancellationToken);

            if (tokenResult.IsSuccess)
            {
                return Results.Ok(new RegisterResponse(tokenResult.Value));
            }

            return ErrorResults.FromError(tokenResult.Error, StatusCodes.Status400BadRequest);
        })
        .WithName("Register")
        .WithSummary("Registers a new user")
        .WithDescription("Creates a new user account with the provided credentials and returns a JWT token.")
        .Produces<RegisterResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
