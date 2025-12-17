using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.UnbanUser;
using Users.Domain.Users;

namespace Users.Presentation.Endpoints;

internal sealed class UnbanUserEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.Unban, async delegate (
            Guid userId,
            HttpContext httpContext,
            ICommandHandler<UnbanUserCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            Guid? unbannedBy = httpContext.GetUserId();
            if (unbannedBy == null)
            {
                return Results.Unauthorized();
            }

            UnbanUserCommand command = new(userId, unbannedBy.Value);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .RequireAuthorization(Permission.UnbanUser.Name)
        .WithName("UnbanUser")
        .WithSummary("Unbans a user")
        .WithDescription("Removes the ban from a user by their user ID.")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
