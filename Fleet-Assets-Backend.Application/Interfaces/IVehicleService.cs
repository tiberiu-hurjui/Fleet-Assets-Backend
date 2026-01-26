using Fleet_Assets_Backend.Application.Dtos.Vehicle;

namespace Fleet_Assets_Backend.Application.Interfaces;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetAllAsync(CancellationToken ct);
    Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<VehicleDto> CreateAsync(CreateVehicleRequest request, Guid correlationId, string? actor, CancellationToken ct);
    Task<VehicleDto?> UpdateAsync(Guid id, UpdateVehicleRequest request, Guid correlationId, string? actor, CancellationToken ct);
    Task<VehicleDto?> ChangeStatusAsync(Guid id, ChangeVehicleStatusRequest request, Guid correlationId, string? actor, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, Guid correlationId, string? actor, CancellationToken ct);
    Task<int> SlowDbDemoAsync(CancellationToken ct);
}