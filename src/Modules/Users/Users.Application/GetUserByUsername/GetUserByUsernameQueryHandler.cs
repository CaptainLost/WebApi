using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Users.Application.Abstractions.Repositories;
using Users.Domain.Entities;
using Users.Domain.Errors;

namespace Users.Application.GetUserByUsername;

internal sealed class GetUserByUsernameQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByUsernameQuery, GetUserResponse>
{
    private readonly IUserRepository m_userRepository = userRepository;

    public async Task<Result<GetUserResponse>> HandleAsync(GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        User? user = await m_userRepository.GetUserByUsernameAsync(query.Username, cancellationToken);

        if (user == null)
        {
            return Result.Failure<GetUserResponse>(UserErrors.NotFound(query.Username));
        }

        GetUserResponse response = new GetUserResponse(
            Id: user.Id,
            Username: user.UserName!);

        return Result.Success(response);
    }
}
