namespace Core.Application.Abstractions.Messaging.Events;

public interface IIntegrationEvent
{
    Guid Id { get; }

    DateTime OccurredAtUtc { get; }
}
