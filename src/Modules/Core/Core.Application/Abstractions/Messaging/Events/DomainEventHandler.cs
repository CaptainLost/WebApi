using Core.Domain.Messaging;

namespace Core.Application.Abstractions.Messaging.Events;

public abstract class DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken = default) =>
        Handle((TDomainEvent)domainEvent, cancellationToken);

    public abstract Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
