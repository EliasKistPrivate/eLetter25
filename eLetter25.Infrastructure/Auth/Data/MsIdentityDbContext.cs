using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eLetter25.Infrastructure.Auth.Data;

public sealed class MsIdentityDbContext(DbContextOptions<MsIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.EnableNotifications).HasDefaultValue(true);
        });

        builder.HasDefaultSchema("identity");
    }
}