using eLetter25.Domain.Common;

namespace eLetter25.Domain.Letters.Events;

public sealed record LetterTagAddedEvent(
    Guid LetterId,
    string Tag) : DomainEventBase;
