using Core.Application.Abstractions.Messaging.Events;
using Core.Domain.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Persistence.DomainEvents;

public sealed class DomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }

    private async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Type domainEventType = domainEvent.GetType();
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEventType);

        using IServiceScope scope = _serviceProvider.CreateScope();

        IEnumerable<IDomainEventHandler> handlers = scope.ServiceProvider
            .GetServices(handlerType)
            .Cast<IDomainEventHandler>();

        foreach (IDomainEventHandler handler in handlers)
        {
            await handler.Handle(domainEvent, cancellationToken);
        }
    }
}
