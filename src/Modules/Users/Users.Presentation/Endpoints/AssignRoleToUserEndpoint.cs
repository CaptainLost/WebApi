using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Enums;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.AssignRoleToUser;

namespace Users.Presentation.Endpoints;

internal sealed class AssignRoleToUserEndpoint : IEndpoint
{
    public sealed record AssignRoleRequest(string RoleName);

    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost(UsersRoutes.AssignRole, async delegate (
            string userId,
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
        .RequireAuthorization(nameof(PermissionType.AssignRole))
        .WithName("AssignRoleToUser")
        .WithSummary("Assigns a role to a user")
        .WithDescription("Assigns a specified role to a user by their user ID.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
