using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.CreateUser;

namespace Users.Presentation.Endpoints;

internal sealed class RegisterEndpoint : IEndpoint
{
    public sealed record RegisterRequest(string Username, string Email, string Password);
    public sealed record RegisterResponse(string Token);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.Register, async delegate (RegisterRequest request,
            ICommandHandler<CreateUserCommand, string> commandHandler,
            CancellationToken cancellationToken)
        {
            CreateUserCommand registerCommand = new(request.Username, request.Email, request.Password);
            Result<string> tokenResult = await commandHandler.HandleAsync(registerCommand, cancellationToken);

            return tokenResult.Match(
                token => Results.Ok(new RegisterResponse(token)),
                ApiResults.Problem);
        })
        .RequireRateLimiting(RateLimiterNames.AuthFixed)
        .WithName("Register")
        .WithSummary("Registers a new user")
        .WithDescription("Creates a new user account with the provided credentials and returns a JWT token.")
        .Produces<RegisterResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
