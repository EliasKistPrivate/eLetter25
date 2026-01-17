using eLetter25.Domain.Common;

namespace eLetter25.Domain.Letters.Events;

public sealed record LetterSubjectChangedEvent(
    Guid LetterId,
    string PreviousSubject,
    string CurrentSubject) : DomainEventBase;
