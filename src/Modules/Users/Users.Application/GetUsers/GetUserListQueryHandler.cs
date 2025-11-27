using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Core.Domain.Pagination;
using Users.Application.Abstractions.Repositories;
using Users.Domain.Entities;

namespace Users.Application.GetUsers;

internal sealed class GetUserListQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserListQuery, GetUserListResponse>
{
    private readonly IUserRepository m_userRepository = userRepository;

    public async Task<Result<GetUserListResponse>> HandleAsync(GetUserListQuery query, CancellationToken cancellationToken)
    {
        PageRequest pageRequest = PageRequest.Create(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        Result<PagedResult<User>> pagedUsersResult = await m_userRepository.GetUsersPagedAsync(pageRequest, cancellationToken);

        if (pagedUsersResult.IsFailure)
        {
            return Result.Failure<GetUserListResponse>(pagedUsersResult.Error);
        }

        PagedResult<User> pagedUsers = pagedUsersResult.Value;

        IReadOnlyCollection<UserDto> userDtos = pagedUsers.Items
            .Select(user => new UserDto(user.Id, user.UserName ?? string.Empty, user.Email))
            .ToList();

        GetUserListResponse response = new GetUserListResponse(userDtos, pagedUsers.Metadata);

        return Result.Success(response);
    }
}
