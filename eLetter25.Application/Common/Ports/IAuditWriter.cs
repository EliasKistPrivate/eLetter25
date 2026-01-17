using eLetter25.Application.Common.Models;

namespace eLetter25.Application.Common.Ports;

public interface IAuditWriter
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
