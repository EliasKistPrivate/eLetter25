using eLetter25.Application.Common.Events;
using eLetter25.Application.Common.Models;
using eLetter25.Application.Common.Ports;
using eLetter25.Domain.Letters.Events;
using MediatR;

namespace eLetter25.Application.Letters.EventHandlers;

public sealed class LetterCreatedAuditHandler(IAuditWriter auditWriter)
    : INotificationHandler<DomainEventNotification<LetterCreatedEvent>>
{
    public Task Handle(
        DomainEventNotification<LetterCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var entry = new AuditEntry(
            "LetterCreated",
            domainEvent.LetterId,
            nameof(LetterCreatedEvent),
            domainEvent.OccurredOn,
            new Dictionary<string, string>
            {
                ["SentDate"] = domainEvent.SentDate.ToString("O"),
                ["CreatedDate"] = domainEvent.CreatedDate.ToString("O"),
            });

        return auditWriter.WriteAsync(entry, cancellationToken);
    }
}
