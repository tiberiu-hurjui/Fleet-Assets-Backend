using Fleet_Assets_Backend.Domain.Entities;
using Fleet_Assets_Backend.Infrastructure.Interfaces;
using Fleet_Assets_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace Fleet_Assets_Backend.Infrastructure.Repositories;

public class VehicleRepository(FleetAssetsDbContext _db, ILogger<VehicleRepository> logger) : IVehicleRepository
{
    private readonly IAsyncPolicy _policy = DbPolicies.CreateReadPolicy(TimeSpan.FromMilliseconds(150), 1, logger);

    public async Task<List<Vehicle>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .OrderBy(v => v.Name)
            .ToListAsync(ct);
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken ct)
    {
        await _db.Vehicles.AddAsync(vehicle, ct);
    }

    public Task DeleteAsync(Vehicle vehicle, CancellationToken ct)
    {
        _db.Vehicles.Remove(vehicle);
        return Task.CompletedTask;
    }

    public Task<int> SlowDbDemoAsync(CancellationToken ct)
    {
        var sql = "WAITFOR DELAY '00:00:01'; SELECT 1;";

        return _policy.ExecuteAsync(async token =>
        {
            await _db.Database.ExecuteSqlRawAsync(sql, token);
            return 1;
        }, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
