using eLetter25.Domain.Common;

namespace eLetter25.Infrastructure.DomainEvents;

public sealed class DomainEventCollector : IDomainEventCollector
{
    private readonly HashSet<DomainEntity> _trackedEntities = [];

    public void CollectFrom(DomainEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (entity.DomainEvents.Count == 0)
        {
            return;
        }

        _trackedEntities.Add(entity);
    }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        return _trackedEntities
            .SelectMany(entity => entity.DomainEvents)
            .ToArray();
    }

    public void Clear()
    {
        foreach (var entity in _trackedEntities)
        {
            entity.ClearDomainEvents();
        }

        _trackedEntities.Clear();
    }
}
