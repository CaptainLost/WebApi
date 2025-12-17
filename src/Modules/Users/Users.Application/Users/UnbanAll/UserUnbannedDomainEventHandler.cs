using Core.Application.Abstractions.Messaging.Events;
using Users.Domain.Users;

namespace Users.Application.Users.UnbanUser;

internal sealed class UserUnbannedDomainEventHandler : DomainEventHandler<UserUnbannedDomainEvent>
{
    public override Task Handle(UserUnbannedDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
