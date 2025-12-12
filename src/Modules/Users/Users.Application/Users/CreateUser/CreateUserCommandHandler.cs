using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Application.Abstractions;

namespace Users.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler(IAccountService authenticationService)
    : ICommandHandler<CreateUserCommand, string>
{
    private readonly IAccountService _accountService = authenticationService;

    public async Task<Result<string>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        Result<string> tokenResult = await _accountService.RegisterAsync(
            command.Username,
            command.Email,
            command.Password,
            cancellationToken);

        return tokenResult;
    }
}
