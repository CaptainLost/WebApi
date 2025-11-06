using Application.Abstractions.Messaging.Commands;
using Domain.Messaging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Abstractions.Messaging.Decorators;

internal sealed class CommandHandlerLoggingDecorator<TCommand>(
    ICommandHandler<TCommand> decorated,
    ILogger<CommandHandlerLoggingDecorator<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> m_decorated = decorated;
    private readonly ILogger<CommandHandlerLoggingDecorator<TCommand>> m_logger = logger;

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;

        m_logger.LogInformation("Executing command: {CommandName}", commandName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Result result = await m_decorated.HandleAsync(command, cancellationToken);

            stopwatch.Stop();

            if (result.IsSuccess)
            {
                m_logger.LogInformation(
                    "Command {CommandName} executed successfully in {ElapsedMilliseconds}ms",
                    commandName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                m_logger.LogWarning(
                    "Command {CommandName} failed in {ElapsedMilliseconds}ms with error: {Error}",
                    commandName,
                    stopwatch.ElapsedMilliseconds,
                    result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            m_logger.LogError(
                ex,
                "Command {CommandName} threw an exception after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

internal sealed class CommandHandlerLoggingDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> decorated,
    ILogger<CommandHandlerLoggingDecorator<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> m_decorated = decorated;
    private readonly ILogger<CommandHandlerLoggingDecorator<TCommand, TResponse>> m_logger = logger;

    public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;

        m_logger.LogInformation("Executing command: {CommandName}", commandName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Result<TResponse> result = await m_decorated.HandleAsync(command, cancellationToken);

            stopwatch.Stop();

            if (result.IsSuccess)
            {
                m_logger.LogInformation(
                    "Command {CommandName} executed successfully in {ElapsedMilliseconds}ms",
                    commandName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                m_logger.LogWarning(
                    "Command {CommandName} failed in {ElapsedMilliseconds}ms with error: {Error}",
                    commandName,
                    stopwatch.ElapsedMilliseconds,
                    result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            m_logger.LogError(
                ex,
                "Command {CommandName} threw an exception after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
