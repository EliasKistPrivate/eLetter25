namespace eLetter25.Domain.Common;

/// <summary>
/// Base type for domain events.
/// </summary>
public abstract record DomainEventBase : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
    public Guid? CorrelationId { get; init; }
}
