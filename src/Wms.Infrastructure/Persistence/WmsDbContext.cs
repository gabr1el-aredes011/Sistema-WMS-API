using Microsoft.EntityFrameworkCore;

namespace Wms.Infrastructure.Persistence;

public sealed class WmsDbContext : DbContext
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WmsDbContext).Assembly);
    }
}