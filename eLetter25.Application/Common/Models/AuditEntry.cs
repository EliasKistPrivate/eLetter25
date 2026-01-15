namespace eLetter25.Application.Common.Models;

public sealed record AuditEntry(
    string Action,
    Guid EntityId,
    string EntityType,
    DateTimeOffset OccurredOn,
    IReadOnlyDictionary<string, string> Metadata);
