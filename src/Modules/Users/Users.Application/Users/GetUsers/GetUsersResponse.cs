using Core.Domain.Pagination;

namespace Users.Application.Users.GetUsers;

public sealed record GetUsersResponse(
    IReadOnlyCollection<UserDto> Items,
    PaginationMetadata Metadata);

public sealed record UserDto(
    Guid Id,
    string Username);
