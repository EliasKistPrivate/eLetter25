using eLetter25.Domain.Common;

namespace eLetter25.Infrastructure.DomainEvents;

public interface IDomainEventCollector
{
    void CollectFrom(DomainEntity entity);
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    void Clear();
}
