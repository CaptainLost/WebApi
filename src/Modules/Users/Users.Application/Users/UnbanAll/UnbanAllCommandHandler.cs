using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Domain.Users;

namespace Users.Application.Users.UnbanAll;

internal sealed class UnbanAllCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UnbanAllCommand>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result> HandleAsync(UnbanAllCommand command, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdWithBansAsync(command.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.UserNotFoundById(command.UserId));
        }

        Result unbanResult = user.UnbanAll(command.UnbannedBy);

        if (unbanResult.IsFailure)
        {
            return unbanResult;
        }

        await _userRepository.SaveChangesAsync();

        return Result.Success();
    }
}
