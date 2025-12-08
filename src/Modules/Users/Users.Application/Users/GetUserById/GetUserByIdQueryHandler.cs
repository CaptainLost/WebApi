using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Users.Domain.Users;

namespace Users.Application.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<GetUserByIdResponse>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(query.userId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<GetUserByIdResponse>(UserErrors.UserNotFoundById(query.userId));
        }

        GetUserByIdResponse response = new GetUserByIdResponse(user.Id, user.Username.Value);

        return Result.Success(response);
    }
}
