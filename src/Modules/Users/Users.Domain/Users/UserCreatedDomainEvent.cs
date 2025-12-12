using Core.Domain.Messaging;
using Users.Domain.ValueObjects;

namespace Users.Domain.Users;

public sealed class UserCreatedDomainEvent : DomainEvent
{
    public UserCreatedDomainEvent(Guid userId, Username username, Email email, Nickname nickname)
    {
        UserId = userId;
        Username = username;
        Email = email;
        Nickname = nickname;
    }

    public Guid UserId { get; }

    public Username Username { get; }

    public Email Email { get; }

    public Nickname Nickname { get; }
}
