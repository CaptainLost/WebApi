using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.Extensions;
using Core.Presentation.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.UnbanSingleUser;
using Users.Domain.Users;

namespace Users.Presentation.Endpoints;

internal sealed class UnbanSingleUserEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete(UsersRoutes.RemoveSingleBan, async delegate (
            Guid userId,
            Guid banId,
            HttpContext httpContext,
            ICommandHandler<RemoveSingleUserBanCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            Guid? banRemoverId = httpContext.GetUserId();
            if (banRemoverId == null)
            {
                return Results.Unauthorized();
            }

            RemoveSingleUserBanCommand command = new(userId, banId, banRemoverId.Value);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .RequireRateLimiting(RateLimiterNames.WriteFixed)
        .RequireAuthorization(Permission.RemoveSingleBan.Name)
        .WithName("RemoveSingleBan")
        .WithSummary("Removes a specific ban from a user")
        .WithDescription("Removes a specific ban from a user by ban ID.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
