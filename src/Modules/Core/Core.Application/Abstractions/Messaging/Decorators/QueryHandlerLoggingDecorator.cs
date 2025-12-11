using System.Diagnostics;
using Core.Application.Abstractions.Messaging.Queries;
using Core.Domain.Messaging;
using Microsoft.Extensions.Logging;

namespace Core.Application.Abstractions.Messaging.Decorators;

internal sealed class QueryHandlerLoggingDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> decorated,
    ILogger<QueryHandlerLoggingDecorator<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _decorated = decorated;
    private readonly ILogger<QueryHandlerLoggingDecorator<TQuery, TResponse>> _logger = logger;

    public async Task<Result<TResponse>> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        string queryName = typeof(TQuery).Name;

        _logger.LogInformation("Executing query: {QueryName}", queryName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Result<TResponse> result = await _decorated.HandleAsync(query, cancellationToken);

            stopwatch.Stop();

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Query {QueryName} executed successfully in {ElapsedMilliseconds}ms",
                    queryName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogWarning(
                    "Query {QueryName} failed in {ElapsedMilliseconds}ms with error: {Error}",
                    queryName,
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
                "Query {QueryName} threw an exception after {ElapsedMilliseconds}ms",
                queryName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
