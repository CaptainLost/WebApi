using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.Extensions;
using Core.Presentation.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.UnbanAll;
using Users.Domain.Users;

namespace Users.Presentation.Endpoints;

internal sealed class UnbanAllEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.RemoveAllUserBans, async delegate (
            Guid userId,
            HttpContext httpContext,
            ICommandHandler<RemoveAllUserBansCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            Guid? banRemoverId = httpContext.GetUserId();
            if (banRemoverId == null)
            {
                return Results.Unauthorized();
            }

            RemoveAllUserBansCommand command = new(userId, banRemoverId.Value);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .RequireRateLimiting(RateLimiterNames.WriteFixed)
        .RequireAuthorization(Permission.RemoveAllUserBans.Name)
        .WithName("RemoveAllUserBans")
        .WithSummary("Removes all bans from a specific user")
        .WithDescription("Deactivates all currently active bans for a specific user by their user ID.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
