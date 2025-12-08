using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.AssignRoleToUser;
using Users.Domain.Users;
using Core.Presentation.Extensions;

namespace Users.Presentation.Endpoints;

internal sealed class AssignRoleToUserEndpoint : IEndpoint
{
    public sealed record AssignRoleRequest(string RoleName);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.AssignRole, async delegate (
            Guid userId,
            AssignRoleRequest request,
            ICommandHandler<AssignRoleToUserCommand> commandHandler,
            CancellationToken cancellationToken)
        {
            AssignRoleToUserCommand command = new(userId, request.RoleName);
            Result result = await commandHandler.HandleAsync(command, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.NoContent();
            }

            return ErrorResults.FromError(result.Error, StatusCodes.Status400BadRequest);
        })
        .RequireAuthorization(Permission.AssignRole)
        .WithName("AssignRoleToUser")
        .WithSummary("Assigns a role to a user")
        .WithDescription("Assigns a specified role to a user by their user ID.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
