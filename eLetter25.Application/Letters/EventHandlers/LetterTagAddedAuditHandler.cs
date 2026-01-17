using eLetter25.Application.Common.Events;
using eLetter25.Application.Common.Models;
using eLetter25.Application.Common.Ports;
using eLetter25.Domain.Letters.Events;
using MediatR;

namespace eLetter25.Application.Letters.EventHandlers;

public sealed class LetterTagAddedAuditHandler(IAuditWriter auditWriter)
    : INotificationHandler<DomainEventNotification<LetterTagAddedEvent>>
{
    public Task Handle(
        DomainEventNotification<LetterTagAddedEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var entry = new AuditEntry(
            "LetterTagAdded",
            domainEvent.LetterId,
            nameof(LetterTagAddedEvent),
            domainEvent.OccurredOn,
            new Dictionary<string, string>
            {
                ["Tag"] = domainEvent.Tag,
            });

        return auditWriter.WriteAsync(entry, cancellationToken);
    }
}
