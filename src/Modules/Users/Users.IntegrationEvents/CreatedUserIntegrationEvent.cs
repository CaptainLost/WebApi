using Core.Application.Abstractions.Messaging.Events;

namespace Users.IntegrationEvents;

public sealed class CreatedUserIntegrationEvent : IIntegrationEvent
{
    public CreatedUserIntegrationEvent(
        Guid id,
        DateTime occurredAtUtc,
        Guid userId,
        string username,
        string email,
        string nickname)
    {
        Id = id;
        OccurredAtUtc = occurredAtUtc;
        UserId = userId;
        Username = username;
        Email = email;
        Nickname = nickname;
    }

    public Guid Id { get; }
    public DateTime OccurredAtUtc { get; }
    public Guid UserId { get; }
    public string Username { get; }
    public string Email { get; }
    public string Nickname { get; }
}
