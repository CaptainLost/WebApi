using Application.Abstractions.Messaging.Commands;
using Domain.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging.InMemoryCommandDispatcher;

internal class InMemoryCommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    private readonly IServiceProvider m_serviceProvider = serviceProvider;

    public async Task<Result> Dispatch<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        using IServiceScope scope = m_serviceProvider.CreateScope();

        ICommandHandler<TCommand> handler = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<TCommand>>();

        if (handler == null)
        {
            return Result.Failure(CommandDispatcherErrors.HandlerNotFound(typeof(TCommand)));
        }

        return await handler.Handle(command, CancellationToken.None);
    }

    public async Task<Result<TResponse>> Dispatch<TCommand, TResponse>(TCommand command)
        where TCommand : class, ICommand<TResponse>
    {
        using IServiceScope scope = m_serviceProvider.CreateScope();

        ICommandHandler<TCommand, TResponse> handler = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        if (handler == null)
        {
            return Result.Failure<TResponse>(CommandDispatcherErrors.HandlerNotFound(typeof(TCommand)));
        }

        return await handler.Handle(command, CancellationToken.None);
    }
}