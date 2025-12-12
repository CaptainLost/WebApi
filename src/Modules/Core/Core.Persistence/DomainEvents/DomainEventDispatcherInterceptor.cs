using Core.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Persistence.DomainEvents;

public sealed class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly DomainEventDispatcher _dispatcher;

    public DomainEventDispatcherInterceptor(DomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var aggregateRoots = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.GetDomainEvents().Any())
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(aggregate => aggregate.GetDomainEvents())
            .ToList();

        aggregateRoots.ForEach(aggregate => aggregate.ClearDomainEvents());

        await _dispatcher.DispatchAsync(domainEvents, cancellationToken);
    }
}
