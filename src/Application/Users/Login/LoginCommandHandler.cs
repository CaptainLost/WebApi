using Application.Abstractions.Messaging.Commands;
using Application.Abstractions.Repositories;
using Domain.Messaging;

namespace Application.Users.Login;

internal sealed class LoginCommandHandler(IUserRepository userRepository) : ICommandHandler<LoginCommand>
{
    private readonly IUserRepository m_userRepository = userRepository;

    public Task<Result> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        // implementacja logiki logowania
        return Task.FromResult(Result.Success());
    }
}
