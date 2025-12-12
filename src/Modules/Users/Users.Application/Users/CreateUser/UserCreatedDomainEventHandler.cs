using Core.Application.Abstractions.Messaging.Events;
using Users.Domain.Users;
using Users.IntegrationEvents;

namespace Users.Application.Users.CreateUser;

internal sealed class UserCreatedDomainEventHandler : DomainEventHandler<UserCreatedDomainEvent>
{
    private readonly IIntegrationEventBus _eventBus;

    public UserCreatedDomainEventHandler(IIntegrationEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var integrationEvent = new CreatedUserIntegrationEvent(
            domainEvent.Id,
            domainEvent.OccurredAtUtc,
            domainEvent.UserId,
            domainEvent.Username.Value,
            domainEvent.Email.Value,
            domainEvent.Nickname.Value);

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
