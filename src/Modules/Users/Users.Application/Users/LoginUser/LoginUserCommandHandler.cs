using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Users.Application.Abstractions;
using Users.Domain.Users;
using Users.Domain.ValueObjects;

namespace Users.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler(
    IAuthenticationService authenticationService, 
    IUserRepository userRepository) 
    : ICommandHandler<LoginUserCommand, string>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<string>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
    {
        Result<Username> username = Username.Create(command.Username);

        User? user = await _userRepository.GetByUsernameAsync(username.Value, cancellationToken);

        if (user == null)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        Result<string> tokenResult = await _authenticationService.LoginAsync(user, command.Password, cancellationToken);

        return tokenResult;
    }
}
