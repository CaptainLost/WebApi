using Authentication.Application.Abstractions.Services;
using Authentication.Domain.Errors;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Entities;
using Core.Domain.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Application.Login;

internal sealed class LoginCommandHandler(
    UserManager<User> userManager,
    IAuthenticationService authenticationService) : ICommandHandler<LoginCommand>
{
    private readonly UserManager<User> m_userManager = userManager;
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await m_userManager.FindByNameAsync(command.Username);

        if (user == null)
        {
            return AuthenticationErrors.LoginFailed();
        }

        Result result = await m_authenticationService.LoginAsync(user, command.Password, command.IsPersistent);

        return result;
    }
}
