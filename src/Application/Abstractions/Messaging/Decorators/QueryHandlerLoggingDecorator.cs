using Application.Abstractions.Messaging.Queries;
using Domain.Messaging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Abstractions.Messaging.Decorators;

internal sealed class QueryHandlerLoggingDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> decorated,
    ILogger<QueryHandlerLoggingDecorator<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> m_decorated = decorated;
    private readonly ILogger<QueryHandlerLoggingDecorator<TQuery, TResponse>> m_logger = logger;

    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        string queryName = typeof(TQuery).Name;

        m_logger.LogInformation("Executing query: {QueryName}", queryName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Result<TResponse> result = await m_decorated.Handle(query, cancellationToken);

            stopwatch.Stop();

            if (result.IsSuccess)
            {
                m_logger.LogInformation(
                    "Query {QueryName} executed successfully in {ElapsedMilliseconds}ms",
                    queryName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                m_logger.LogWarning(
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

            m_logger.LogError(
                ex,
                "Query {QueryName} threw an exception after {ElapsedMilliseconds}ms",
                queryName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
