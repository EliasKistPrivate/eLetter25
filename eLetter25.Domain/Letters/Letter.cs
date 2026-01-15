using eLetter25.Domain.Common;
using eLetter25.Domain.Letters.Events;
using eLetter25.Domain.Letters.ValueObjects;

namespace eLetter25.Domain.Letters;

/// <summary>
/// Represents a letter with its details and correspondents.
/// </summary>
public class Letter : DomainEntity
{
    public string Subject { get; private set; } = string.Empty;

    public IReadOnlyCollection<Tag> Tags { get; private set; } = [];

    public DateTimeOffset SentDate { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; }

    public Correspondent? Sender { get; private set; }
    public Correspondent? Recipient { get; private set; }

    public Guid? SenderReferenceId { get; private set; }
    public Guid? RecipientReferenceId { get; private set; }

    private Letter() // For EF Core
    {
    }

    public static Letter Create(Correspondent sender, Correspondent recipient, DateTimeOffset sentDate)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(recipient);

        if (sentDate == default)
        {
            throw new ArgumentException("Sent date must be a valid date.", nameof(sentDate));
        }

        var letter = new Letter
        {
            Sender = sender,
            Recipient = recipient,
            SentDate = sentDate,
            CreatedDate = DateTimeOffset.UtcNow,
        };
        letter.Raise(new LetterCreatedEvent(letter.Id, letter.SentDate, letter.CreatedDate));
        return letter;
    }

    public Letter SetSubject(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Subject cannot be null or whitespace.", nameof(subject));
        }

        var normalizedSubject = subject.Trim();
        if (string.Equals(Subject, normalizedSubject, StringComparison.Ordinal))
        {
            return this;
        }

        var previousSubject = Subject;
        Subject = normalizedSubject;
        Raise(new LetterSubjectChangedEvent(Id, previousSubject, Subject));
        return this;
    }

    public Letter SetSenderReferenceId(Guid senderReferenceId)
    {
        SenderReferenceId = senderReferenceId;
        return this;
    }

    public Letter SetRecipientReferenceId(Guid recipientReferenceId)
    {
        RecipientReferenceId = recipientReferenceId;
        return this;
    }


    public Letter AddTag(Tag tag)
    {
        var tags = Tags.ToList();
        if (tags.Any(existingTag => existingTag.Equals(tag)))
        {
            return this;
        }

        tags.Add(tag);
        Tags = tags;
        Raise(new LetterTagAddedEvent(Id, tag.Value));
        return this;
    }

    public Letter AddTags(IEnumerable<Tag> tags)
    {
        foreach (var tag in tags)
        {
            AddTag(tag);
        }
        return this;
    }


    public void ClearTags()
    {
        Tags = [];
    }

    public void RemoveTag(Tag tag)
    {
        var tags = Tags.ToList();
        tags.Remove(tag);
        Tags = tags;
    }

    public void RemoveTag(string tagName)
    {
        var tags = Tags.ToList();
        var tagToRemove = tags.FirstOrDefault(t => t.Value == tagName);

        tags.Remove(tagToRemove);
        Tags = tags;
    }

    public bool HasTag(string tagName)
    {
        return Tags.Any(t => t.Value == tagName);
    }
}
