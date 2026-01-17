namespace eLetter25.Domain.Common;

/// <summary>
/// Represents a domain event emitted by aggregate roots.
/// </summary>
public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
    Guid? CorrelationId { get; }
}
