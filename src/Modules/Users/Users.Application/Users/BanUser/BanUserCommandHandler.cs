using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Domain.Users;

namespace Users.Application.Users.BanUser;

internal sealed class BanUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<BanUserCommand>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result> HandleAsync(BanUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdWithBansAsync(command.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(UserErrors.UserNotFoundById(command.UserId));
        }

        Result banResult = user.Ban(command.Reason, command.BanImposerId, command.ExpiresAt);

        if (banResult.IsFailure)
        {
            return banResult;
        }

        await _userRepository.SaveChangesAsync();

        return Result.Success();
    }
}
