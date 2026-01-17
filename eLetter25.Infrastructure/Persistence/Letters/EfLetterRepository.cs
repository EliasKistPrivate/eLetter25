using eLetter25.Application.Letters.Ports;
using eLetter25.Domain.Letters;
using eLetter25.Infrastructure.DomainEvents;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using Microsoft.EntityFrameworkCore;

namespace eLetter25.Infrastructure.Persistence.Letters;

public sealed class EfLetterRepository(
    AppDbContext dbContext,
    ILetterDomainToDbMapper domainToDbMapper,
    ILetterDbToDomainMapper dbToDomainMapper,
    IDomainEventCollector domainEventCollector) : ILetterRepository
{
    public async Task AddAsync(Letter letter, CancellationToken cancellationToken = default)
    {
        var entity = domainToDbMapper.MapToDbEntity(letter);
        await dbContext.Letters.AddAsync(entity, cancellationToken);
        domainEventCollector.CollectFrom(letter);
    }

    public async Task<Letter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Letters
            .Include(l => l.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        return entity is null ? null : dbToDomainMapper.MapToDomain(entity);
    }
}
