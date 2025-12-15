using Core.Domain.Messaging;

namespace Users.Domain.Users;

public sealed record UserUnbannedDomainEvent(Guid UserId) : DomainEvent;
