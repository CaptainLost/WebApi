using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.BanUser;
using Users.Domain.Users;

namespace Users.Presentation.Endpoints;

internal sealed class BanUserEndpoint : IEndpoint
{
    public sealed record BanUserRequest(string Reason, int DurationInSeconds);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.Ban, async delegate (
            Guid userId,
            BanUserRequest request,
            HttpContext httpContext,
            ICommandHandler<BanUserCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            Guid? bannedBy = httpContext.GetUserId();
            
            if (bannedBy == null)
            {
                return Results.Unauthorized();
            }

            DateTime expiresAt = DateTime.UtcNow.AddSeconds(request.DurationInSeconds);
            
            BanUserCommand command = new(userId, request.Reason, bannedBy.Value, expiresAt);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .RequireAuthorization(Permission.BanUser.Name)
        .WithName("BanUser")
        .WithSummary("Bans a user")
        .WithDescription("Bans a user by their user ID with a specified reason and duration in seconds.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
