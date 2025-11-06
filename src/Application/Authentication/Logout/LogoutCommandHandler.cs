using Application.Abstractions.Messaging.Commands;
using Application.Abstractions.Services;
using Domain.Messaging;

namespace Application.Authentication.Logout;

internal sealed class LogoutCommandHandler(
    IAuthenticationService authenticationService) : ICommandHandler<LogoutCommand>
{
    private readonly IAuthenticationService m_authenticationService = authenticationService;

    public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken cancellationToken)
    {
        await m_authenticationService.LogoutAsync();

        return Result.Success();
    }
}
