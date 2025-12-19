using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Core.Presentation.Common;
using Core.Presentation.Endpoints;
using Core.Presentation.Models;
using Core.Presentation.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.GetUsers;
using Users.Domain.Users;

namespace Users.Presentation.Endpoints;

internal sealed class GetUserListEndpoint : IEndpoint
{
    public void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet(UsersRoutes.GetUserList, async (
            IQueryHandler<GetUsersQuery, GetUsersResponse> queryHandler,
            [AsParameters] PaginationQueryParameters pagination,
            CancellationToken cancellationToken) =>
        {
            GetUsersQuery query = new GetUsersQuery(
                pagination.PageNumber,
                pagination.PageSize,
                pagination.SearchTerm,
                pagination.SortBy,
                pagination.SortDescending);

            Result<GetUsersResponse> result = await queryHandler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireRateLimiting(RateLimiterNames.ReadFixed)
        .RequireAuthorization(Permission.GetUserList.Name)
        .WithName("GetUserList")
        .WithSummary("Get paginated list of users with filtering and sorting")
        .WithDescription("Retrieves a paginated list of users with optional filtering by username and sorting capabilities")
        .Produces<GetUsersResponse>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }
}
