using eLetter25.Domain.Common;

namespace eLetter25.Domain.Letters.Events;

public sealed record LetterCreatedEvent(
    Guid LetterId,
    DateTimeOffset SentDate,
    DateTimeOffset CreatedDate) : DomainEventBase;
