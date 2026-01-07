namespace eLetter25.Application.Common.Ports;

/// <summary>
/// Represents a unit of work for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commits all changes made within the unit of work to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}