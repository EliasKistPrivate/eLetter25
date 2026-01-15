using eLetter25.Application.Common.Models;
using eLetter25.Application.Common.Ports;
using Microsoft.Extensions.Logging;

namespace eLetter25.Infrastructure.Observability;

public sealed class LoggerAuditWriter(ILogger<LoggerAuditWriter> logger) : IAuditWriter
{
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Audit event {Action} for {EntityType} {EntityId} at {OccurredOn} with {Metadata}.",
            entry.Action,
            entry.EntityType,
            entry.EntityId,
            entry.OccurredOn,
            entry.Metadata);

        return Task.CompletedTask;
    }
}
