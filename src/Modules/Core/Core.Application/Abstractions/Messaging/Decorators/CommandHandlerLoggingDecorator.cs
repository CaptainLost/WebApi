using System.Diagnostics;
using Core.Application.Abstractions.Messaging.Commands;
using Core.Domain.Messaging;
using Microsoft.Extensions.Logging;

namespace Core.Application.Abstractions.Messaging.Decorators;

internal sealed class CommandHandlerLoggingDecorator<TCommand>(
    ICommandHandler<TCommand> decorated,
    ILogger<CommandHandlerLoggingDecorator<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _decorated = decorated;
    private readonly ILogger<CommandHandlerLoggingDecorator<TCommand>> _logger = logger;

    public async Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;

        _logger.LogInformation("Executing command: {CommandName}", commandName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Result result = await _decorated.HandleAsync(command, cancellationToken);

            stopwatch.Stop();

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Command {CommandName} executed successfully in {ElapsedMilliseconds}ms",
                    commandName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
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

            _logger.LogError(
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
    private readonly ICommandHandler<TCommand, TResponse> _decorated = decorated;
    private readonly ILogger<CommandHandlerLoggingDecorator<TCommand, TResponse>> _logger = logger;

    public async Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;

        _logger.LogInformation("Executing command: {CommandName}", commandName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Result<TResponse> result = await _decorated.HandleAsync(command, cancellationToken);

            stopwatch.Stop();

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Command {CommandName} executed successfully in {ElapsedMilliseconds}ms",
                    commandName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
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

            _logger.LogError(
                ex,
                "Command {CommandName} threw an exception after {ElapsedMilliseconds}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
