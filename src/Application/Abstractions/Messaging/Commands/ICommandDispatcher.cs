using Domain.Messaging;

namespace Application.Abstractions.Messaging.Commands;

public interface ICommandDispatcher
{
    Task<Result> Dispatch<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    Task<Result<TResponse>> Dispatch<TCommand, TResponse>(TCommand command)
        where TCommand : class, ICommand<TResponse>;
}