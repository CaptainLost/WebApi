using Application.Abstractions.Messaging.Commands;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Errors;
using Domain.Messaging;
using Domain.Users;

namespace Application.Authentication.Register;

internal sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IAuthenticationService authenticationService) : ICommandHandler<RegisterCommand>
{
    private readonly IUserRepository m_userRepository = userRepository;
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result> HandleAwait(RegisterCommand command, CancellationToken cancellationToken)
    {
        Result registrationResult = await m_authenticationService.RegisterAsync(command.Username, command.Email, command.Password);

        if (registrationResult.IsFailure)
        {
            return registrationResult;
        }

        User? user = await m_userRepository.GetUserByUsernameAsync(command.Username);

        if (user == null)
        {
            return AuthenticationErrors.LoginFailed();
        }

        Result loginResult = await m_authenticationService.LoginAsync(user, command.Password);

        return loginResult;
    }
}
