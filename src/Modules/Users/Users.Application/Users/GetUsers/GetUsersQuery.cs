using Core.Application.Abstractions.Messaging.Queries;

namespace Users.Application.Users.GetUsers;

public sealed record GetUsersQuery(
    int? PageNumber = null,
    int? PageSize = null,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false) : IQuery<GetUsersResponse>;
