using eLetter25.Domain.Common;

namespace eLetter25.Infrastructure.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
