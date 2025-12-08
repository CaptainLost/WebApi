using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.LoginUser;

namespace Users.Presentation.Endpoints;

internal sealed class LoginEndpoint : IEndpoint
{
    public sealed record LoginRequest(string Username, string Password);
    public sealed record LoginResponse(string Token);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.Login, async delegate (LoginRequest request,
            ICommandHandler<LoginUserCommand, string> commandHandler,
            CancellationToken cancellationToken)
        {
            LoginUserCommand loginCommand = new(request.Username, request.Password);
            Result<string> tokenResult = await commandHandler.HandleAsync(loginCommand, cancellationToken);

            if (tokenResult.IsSuccess)
            {
                return Results.Ok(new LoginResponse(tokenResult.Value));
            }

            return ErrorResults.FromError(tokenResult.Error, StatusCodes.Status401Unauthorized);
        })
        .WithName("Login")
        .WithSummary("Authenticates a user")
        .WithDescription("Authenticates a user with username and password. Returns a JWT token upon success.")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized);
    }
}
