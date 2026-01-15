using eLetter25.Application.Common.Ports;
using eLetter25.Infrastructure.DomainEvents;

namespace eLetter25.Infrastructure.Persistence;

public sealed class EfUnitOfWork(
    AppDbContext dbContext,
    IDomainEventCollector domainEventCollector,
    IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = domainEventCollector.GetDomainEvents();
        await dbContext.SaveChangesAsync(cancellationToken);

        if (domainEvents.Count == 0)
        {
            return;
        }

        await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        domainEventCollector.Clear();
    }
}
