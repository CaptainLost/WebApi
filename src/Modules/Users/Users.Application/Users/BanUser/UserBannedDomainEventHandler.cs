using Core.Application.Abstractions.Messaging.Events;
using Users.Domain.Users;

namespace Users.Application.Users.BanUser;

internal sealed class UserBannedDomainEventHandler : DomainEventHandler<UserBannedDomainEvent>
{
    public override Task Handle(UserBannedDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
