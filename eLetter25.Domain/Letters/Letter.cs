using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.ValueObjects;

namespace eLetter25.Domain.Letters;

/// <summary>
/// Represents a letter with its details and correspondents.
/// </summary>
public class Letter(Correspondent sender, Correspondent recipient) : DomainEntity
{
    public string Subject { get; set; } = string.Empty;

    public IReadOnlyCollection<Tag> Tags { get; set; } = [];

    public DateTimeOffset SentDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public Correspondent Sender { get; private set; } = sender;
    public Correspondent Recipient { get; private set; } = recipient;

    public Guid? SenderReferenceId { get; set; }
    public Guid? RecipientReferenceId { get; set; }
}