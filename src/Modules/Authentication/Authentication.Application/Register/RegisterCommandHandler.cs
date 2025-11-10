using Authentication.Application.Abstractions.Services;
using Authentication.Domain.Errors;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Entities;
using Core.Domain.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Register;

internal sealed class RegisterCommandHandler(
    UserManager<User> userManager,
    IAuthenticationService authenticationService) : ICommandHandler<RegisterCommand>
{
    private readonly UserManager<User> m_userManager = userManager;
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        Result registrationResult = await m_authenticationService.RegisterAsync(command.Username, command.Email, command.Password);

        if (registrationResult.IsFailure)
        {
            return registrationResult;
        }

        User? user = await m_userManager.FindByNameAsync(command.Username);

        if (user == null)
        {
            return AuthenticationErrors.LoginFailed();
        }

        Result loginResult = await m_authenticationService.LoginAsync(user, command.Password, isPersistent: false);

        return loginResult;
    }
}
