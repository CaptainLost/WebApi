using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Enums;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.GetUsers;

namespace Users.Presentation.Endpoints;

internal sealed class GetUserListEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet(UsersRoutes.GetUserList, async (
            IQueryHandler<GetUserListQuery, GetUserListResponse> queryHandler,
            [AsParameters] PaginationQueryParameters pagination,
            CancellationToken cancellationToken) =>
        {
            GetUserListQuery query = new GetUserListQuery(
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchTerm,
                pagination.SortBy,
                pagination.SortDescending);

            Result<GetUserListResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }

            return ErrorResults.FromError(result.Error, StatusCodes.Status404NotFound);
        })
        .RequireAuthorization(nameof(PermissionType.ReadUserList))
        .WithName("GetUserList")
        .WithSummary("Get paginated list of users with filtering and sorting")
        .WithDescription("Retrieves a paginated list of users with optional filtering by username and sorting capabilities")
        .Produces<GetUserListResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}