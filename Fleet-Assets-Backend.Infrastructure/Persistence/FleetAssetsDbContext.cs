using Fleet_Assets_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fleet_Assets_Backend.Infrastructure.Persistence;

public class FleetAssetsDbContext(DbContextOptions<FleetAssetsDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<EventLog> EventLogs => Set<EventLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetAssetsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
