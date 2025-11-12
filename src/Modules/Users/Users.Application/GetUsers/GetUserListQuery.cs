using Core.Application.Abstractions.Messaging.Queries;

namespace Users.Application.GetUsers;

public sealed record GetUserListQuery(
    int? PageNumber = null,
    int? PageSize = null,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false) : IQuery<GetUserListResponse>;
