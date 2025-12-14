using Core.Domain.Messaging;
using Users.Domain.ValueObjects;

namespace Users.Domain.Users;

public sealed record UserCreatedDomainEvent(Guid UserId, Username Username, Email Email, Nickname Nickname)
    : DomainEvent;
