using Authentication.Application.Abstractions.Services;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;

namespace Authentication.Application.Logout;

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
