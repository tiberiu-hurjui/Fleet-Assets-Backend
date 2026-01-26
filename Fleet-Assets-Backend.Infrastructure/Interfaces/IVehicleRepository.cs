using Fleet_Assets_Backend.Domain.Entities;

namespace Fleet_Assets_Backend.Infrastructure.Interfaces;
public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync(CancellationToken ct);
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Vehicle vehicle, CancellationToken ct);
    Task DeleteAsync(Vehicle vehicle, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<int> SlowDbDemoAsync(CancellationToken ct);
}