using Application.Abstractions.Messaging.Queries;
using Application.Abstractions.Repositories;
using Domain.Errors;
using Domain.Messaging;
using Domain.Users;

namespace Application.Users.GetUserByUsername;

internal sealed class GetUserByUsernameQueryHandler(
    IUserRepository userRepository) : IQueryHandler<GetUserByUsernameQuery, UserResponse>
{
    private readonly IUserRepository m_userRepository = userRepository;

    public async Task<Result<UserResponse>> HandleAsync(GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        User? user = await m_userRepository.GetUserByUsernameAsync(query.Username);

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
