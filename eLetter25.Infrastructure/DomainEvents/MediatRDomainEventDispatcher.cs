using eLetter25.Application.Common.Events;
using eLetter25.Domain.Common;
using MediatR;

namespace eLetter25.Infrastructure.DomainEvents;

public sealed class MediatRDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
            await mediator.Publish(notification, cancellationToken);
        }
    }
}
