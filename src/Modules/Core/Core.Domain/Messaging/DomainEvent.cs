namespace Core.Domain.Messaging;

public abstract record DomainEvent : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;

    protected DomainEvent()
    {
    }

    protected DomainEvent(Guid id, DateTime occurredAtUtc)
    {
        Id = id;
        OccurredAtUtc = occurredAtUtc;
    }
}
