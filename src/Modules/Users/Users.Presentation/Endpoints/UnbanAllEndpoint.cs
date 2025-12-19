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
        group.MapPost(UsersRoutes.UnbanAll, async delegate (
            Guid userId,
            HttpContext httpContext,
            ICommandHandler<UnbanAllCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            Guid? unbannedBy = httpContext.GetUserId();
            if (unbannedBy == null)
            {
                return Results.Unauthorized();
            }

            UnbanAllCommand command = new(userId, unbannedBy.Value);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .RequireRateLimiting(RateLimiterNames.WriteFixed)
        .RequireAuthorization(Permission.UnbanAllBans.Name)
        .WithName("UnbanAllBans")
        .WithSummary("Removes all active bans from a user")
        .WithDescription("Deactivates all currently active bans for a specific user by their user ID.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
