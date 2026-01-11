using eLetter25.Infrastructure.Persistence.Letters;
using Microsoft.EntityFrameworkCore;

namespace eLetter25.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LetterDbEntity> Letters { get; set; } = null!;
    public DbSet<LetterTagDbEntity> LetterTags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("eletter25");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}