using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.Events;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;
using FluentAssertions;

namespace eLetter25.Domain.Tests.Letters;

public sealed class LetterDomainEventTests
{
    [Fact]
    public void Create_ShouldRaiseLetterCreatedEvent()
    {
        var sender = CreateCorrespondent("Sender");
        var recipient = CreateCorrespondent("Recipient");

        var letter = Letter.Create(sender, recipient, DateTimeOffset.UtcNow);

        letter.DomainEvents.Should().ContainSingle(@event =>
            @event is LetterCreatedEvent created &&
            created.LetterId == letter.Id &&
            created.SentDate == letter.SentDate &&
            created.CreatedDate == letter.CreatedDate);
    }

    [Fact]
    public void AddTag_ShouldRaiseEventOnceAndPreventDuplicates()
    {
        var sender = CreateCorrespondent("Sender");
        var recipient = CreateCorrespondent("Recipient");
        var letter = Letter.Create(sender, recipient, DateTimeOffset.UtcNow);
        letter.ClearDomainEvents();

        var tag = new Tag("Urgent");
        letter.AddTag(tag);
        letter.AddTag(tag);

        letter.Tags.Should().ContainSingle(t => t.Equals(tag));
        letter.DomainEvents.Should().ContainSingle(@event =>
            @event is LetterTagAddedEvent tagEvent &&
            tagEvent.LetterId == letter.Id &&
            tagEvent.Tag == tag.Value);
    }

    private static Correspondent CreateCorrespondent(string name)
    {
        return new Correspondent(
            name,
            new Address("Main Street", "12345", "Berlin", "DE"));
    }
}
