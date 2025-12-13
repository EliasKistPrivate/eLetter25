namespace eLetter25.Domain.Common;

/// <summary>
/// Base entity class with a unique identifier.
/// </summary>
public abstract class DomainEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}