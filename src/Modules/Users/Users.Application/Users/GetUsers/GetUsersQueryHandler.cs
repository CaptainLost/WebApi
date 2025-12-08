using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Core.Domain.Pagination;
using Users.Domain.Users;

namespace Users.Application.Users.GetUsers;

internal sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<GetUsersResponse>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken)
    {
        PageRequest pageRequest = PageRequest.Create(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        (IEnumerable<User> users, int totalCount) = await _userRepository.GetPagedAsync(pageRequest, cancellationToken);

        IReadOnlyCollection<UserDto> userDtos = users
            .Select(user => new UserDto(user.Id, user.Username.Value))
            .ToList();

        Result<PaginationMetadata> metadataResult = PaginationMetadata.Create(
            totalCount,
            pageRequest.PageNumber,
            pageRequest.PageSize);

        if (metadataResult.IsFailure)
        {
            return Result.Failure<GetUsersResponse>(metadataResult.Error);
        }

        GetUsersResponse response = new(userDtos, metadataResult.Value);

        return Result.Success(response);
    }
}
