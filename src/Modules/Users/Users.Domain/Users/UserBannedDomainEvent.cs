using Core.Domain.Messaging;

namespace Users.Domain.Users;

public sealed record UserBannedDomainEvent(
    Guid UserId,
    string Reason,
    Guid BannedBy,
    DateTime ExpiresAt) : DomainEvent;
