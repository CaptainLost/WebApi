using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Application.Abstractions;

namespace Users.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(IAuthenticationService authenticationService) 
    : ICommandHandler<RegisterUserCommand, string>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<string>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        Result<string> tokenResult = await _authenticationService.RegisterAsync(
            command.Username, 
            command.Email, 
            command.Password,
            cancellationToken);

        return tokenResult;
    }
}
