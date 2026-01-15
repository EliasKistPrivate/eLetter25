using eLetter25.Domain.Letters;
using eLetter25.Domain.Letters.ValueObjects;
using eLetter25.Domain.Shared.ValueObjects;
using eLetter25.Infrastructure.DomainEvents;
using eLetter25.Infrastructure.Persistence;
using eLetter25.Infrastructure.Persistence.Letters;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace eLetter25.Infrastructure.Tests.DomainEvents;

public sealed class DomainEventDispatchTests
{
    [Fact]
    public async Task CommitAsync_ShouldDispatchDomainEventsAfterSaveChanges()
    {
        var dispatcher = new TestDomainEventDispatcher();
        var collector = new DomainEventCollector();
        await using var dbContext = BuildDbContext();
        var repository = new EfLetterRepository(
            dbContext,
            new LetterDomainToDbMapper(),
            new LetterDbToDomainMapper(),
            collector);
        var unitOfWork = new EfUnitOfWork(dbContext, collector, dispatcher);

        var letter = Letter.Create(CreateCorrespondent("Sender"), CreateCorrespondent("Recipient"), DateTimeOffset.UtcNow);

        await repository.AddAsync(letter);
        await unitOfWork.CommitAsync();

        dispatcher.DispatchedEvents.Should().HaveCount(1);
        dispatcher.DispatchedEvents.Single().LetterId.Should().Be(letter.Id);
    }

    [Fact]
    public async Task CommitAsync_ShouldNotDispatch_WhenSaveChangesFails()
    {
        var dispatcher = new TestDomainEventDispatcher();
        var collector = new DomainEventCollector();
        await using var dbContext = BuildDbContext(new FailingSaveChangesInterceptor());
        var repository = new EfLetterRepository(
            dbContext,
            new LetterDomainToDbMapper(),
            new LetterDbToDomainMapper(),
            collector);
        var unitOfWork = new EfUnitOfWork(dbContext, collector, dispatcher);

        var letter = Letter.Create(CreateCorrespondent("Sender"), CreateCorrespondent("Recipient"), DateTimeOffset.UtcNow);
        await repository.AddAsync(letter);

        var action = () => unitOfWork.CommitAsync();

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Simulated failure");
        dispatcher.DispatchedEvents.Should().BeEmpty();
    }

    private static AppDbContext BuildDbContext(SaveChangesInterceptor? interceptor = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"eletter25-{Guid.NewGuid()}");

        if (interceptor is not null)
        {
            optionsBuilder.AddInterceptors(interceptor);
        }

        return new AppDbContext(optionsBuilder.Options);
    }

    private static Correspondent CreateCorrespondent(string name)
    {
        return new Correspondent(
            name,
            new Address("Main Street", "12345", "Berlin", "DE"));
    }

    private sealed class FailingSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated failure");
        }
    }

    private sealed class TestDomainEventDispatcher : IDomainEventDispatcher
    {
        public List<Domain.Letters.Events.LetterCreatedEvent> DispatchedEvents { get; } = [];

        public Task DispatchAsync(
            IReadOnlyCollection<Domain.Common.IDomainEvent> domainEvents,
            CancellationToken cancellationToken = default)
        {
            DispatchedEvents.AddRange(domainEvents.OfType<Domain.Letters.Events.LetterCreatedEvent>());
            return Task.CompletedTask;
        }
    }
}
