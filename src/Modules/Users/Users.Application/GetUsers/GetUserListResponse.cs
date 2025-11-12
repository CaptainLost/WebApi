namespace Users.Application.GetUsers;

using Core.Domain.Pagination;

public sealed record GetUserListResponse(IReadOnlyCollection<UserDto> Items, PaginationMetadata Metadata);

public sealed record UserDto(
    string Id,
    string Username,
    string? Email);
