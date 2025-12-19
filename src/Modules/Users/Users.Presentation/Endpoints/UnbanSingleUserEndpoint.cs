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
        group.MapDelete(UsersRoutes.UnbanSingle, async delegate (
            Guid userId,
            Guid banId,
            HttpContext httpContext,
            ICommandHandler<UnbanSingleUserCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            Guid? unbannedBy = httpContext.GetUserId();
            if (unbannedBy == null)
            {
                return Results.Unauthorized();
            }

            UnbanSingleUserCommand command = new(userId, banId, unbannedBy.Value);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .RequireRateLimiting(RateLimiterNames.WriteFixed)
        .RequireAuthorization(Permission.UnbanSingleBan.Name)
        .WithName("UnbanSingleUser")
        .WithSummary("Unbans a single ban from a user")
        .WithDescription("Removes a specific ban from a user by ban ID.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
