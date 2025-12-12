using Core.Application.Abstractions.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.EventBus;

internal sealed class InMemoryIntegrationEventBus : IIntegrationEventBus
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryIntegrationEventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IIntegrationEvent
    {
        Type eventType = typeof(TIntegrationEvent);
        Type handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

        using IServiceScope scope = _serviceProvider.CreateScope();

        List<IIntegrationEventHandler> handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .Cast<IIntegrationEventHandler>()
            .ToList();

        foreach (IIntegrationEventHandler handler in handlers)
        {
            await handler.Handle(integrationEvent, cancellationToken);
        }
    }
}
