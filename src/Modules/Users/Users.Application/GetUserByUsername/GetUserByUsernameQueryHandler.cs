using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Entities;
using Core.Domain.Messaging;
using Users.Application.Abstractions.Repositories;
using Users.Domain.Errors;

namespace Users.Application.GetUserByUsername;

internal sealed class GetUserByUsernameQueryHandler(
    IUserRepository userRepository) : IQueryHandler<GetUserByUsernameQuery, UserResponse>
{
    private readonly IUserRepository m_userRepository = userRepository;

    public async Task<Result<UserResponse>> HandleAsync(GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        User? user = await m_userRepository.GetUserByUsernameAsync(query.Username, cancellationToken);

        if (user == null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.Username));
        }

        UserResponse response = new UserResponse(
            Id: user.Id,
            Username: user.UserName!);

        return Result.Success(response);
    }
}
