using Fleet_Assets_Backend.Domain.Entities;
using Fleet_Assets_Backend.Infrasturcture.Interfaces;
using Fleet_Assets_Backend.Infrasturcture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fleet_Assets_Backend.Infrasturcture.Repositories;

public class VehicleRepository(FleetAssetsDbContext db) : IVehicleRepository
{
    private readonly FleetAssetsDbContext _db = db;

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

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
