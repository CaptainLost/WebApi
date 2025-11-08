using Application.Abstractions.Messaging.Commands;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Messaging;

namespace Application.Authentication.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IAuthenticationService authenticationService) : ICommandHandler<LoginCommand>
{
    private readonly IUserRepository m_userRepository = userRepository;
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await m_userRepository.GetUserByUsernameAsync(command.Username, cancellationToken);

        if (user == null)
        {
            return AuthenticationErrors.LoginFailed();
        }

        Result result = await m_authenticationService.LoginAsync(user, command.Password, command.IsPersistent);

        return result;
    }
}
