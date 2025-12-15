using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Domain.Users;

namespace Users.Application.Users.UnbanUser;

internal sealed class UnbanUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UnbanUserCommand>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result> HandleAsync(UnbanUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.UserNotFoundById(command.UserId));
        }

        Result unbanResult = user.Unban();

        if (unbanResult.IsFailure)
        {
            return unbanResult;
        }

        await _userRepository.SaveChangesAsync();

        return Result.Success();
    }
}
