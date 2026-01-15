using Fleet_Assets_Backend.Application.Dtos.Vehicle;
using Fleet_Assets_Backend.Application.Interfaces;
using Fleet_Assets_Backend.Domain.Entities;
using Fleet_Assets_Backend.Domain.Enums;

using Fleet_Assets_Backend.Infrastructure.Interfaces;
using System.Text.Json;

namespace Fleet_Assets_Backend.Application.Services;

public class VehicleService(IVehicleRepository vehicles, IEventLogRepository events) : IVehicleService
{
    private readonly IVehicleRepository _vehicles = vehicles;
    private readonly IEventLogRepository _events = events;

    public async Task<List<VehicleDto>> GetAllAsync(CancellationToken ct)
        => [.. (await _vehicles.GetAllAsync(ct)).Select(ToDto)];

    public async Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var v = await _vehicles.GetByIdAsync(id, ct);
        return v is null ? null : ToDto(v);
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request, Guid correlationId, string? actor, CancellationToken ct)
    {
        var vehicle = new Vehicle(request.Name, request.Type, request.LastKnownLocation);

        await _vehicles.AddAsync(vehicle, ct);

        await _events.AddAsync(new EventLog(
            eventType: EventType.VehicleCreated,
            entityType: "Vehicle",
            entityId: vehicle.Id,
            correlationId: correlationId,
            severity: EventSeverity.Info,
            summary: $"Vehicle created: {vehicle.Name} ({vehicle.Type})",
            dataJson: JsonSerializer.Serialize(new { after = new { vehicle.Id, vehicle.Name, vehicle.Type, Status = vehicle.Status.ToString(), vehicle.LastKnownLocation } }),
            source: "FleetAssets.Api",
            actor: actor ?? "system"
        ), ct);

        await _vehicles.SaveChangesAsync(ct);
        await _events.SaveChangesAsync(ct);

        return ToDto(vehicle);
    }

    public async Task<VehicleDto?> UpdateAsync(Guid id, UpdateVehicleRequest request, Guid correlationId, string? actor, CancellationToken ct)
    {
        var vehicle = await _vehicles.GetByIdAsync(id, ct);
        if (vehicle is null) return null;

        var before = new { vehicle.Name, vehicle.Type, Status = vehicle.Status.ToString(), vehicle.LastKnownLocation };

        vehicle.UpdateDetails(request.Name, request.Type, request.LastKnownLocation);

        var after = new { vehicle.Name, vehicle.Type, Status = vehicle.Status.ToString(), vehicle.LastKnownLocation };

        await _events.AddAsync(new EventLog(
            EventType.VehicleUpdated,
            "Vehicle",
            vehicle.Id,
            correlationId,
            EventSeverity.Info,
            $"Vehicle updated: {vehicle.Name} ({vehicle.Type})",
            JsonSerializer.Serialize(new { before, after }),
            "FleetAssets.Api",
            actor ?? "system"
        ), ct);

        await _vehicles.SaveChangesAsync(ct);
        await _events.SaveChangesAsync(ct);

        return ToDto(vehicle);
    }

    public async Task<VehicleDto?> ChangeStatusAsync(Guid id, ChangeVehicleStatusRequest request, Guid correlationId, string? actor, CancellationToken ct)
    {
        var vehicle = await _vehicles.GetByIdAsync(id, ct);
        if (vehicle is null) return null;

        if (!Enum.TryParse<VehicleStatus>(request.Status, ignoreCase: true, out var newStatus))
            throw new ArgumentException($"Invalid status: {request.Status}");

        var before = vehicle.Status.ToString();
        vehicle.ChangeStatus(newStatus);
        var after = vehicle.Status.ToString();

        await _events.AddAsync(new EventLog(
            EventType.VehicleStatusChanged,
            "Vehicle",
            vehicle.Id,
            correlationId,
            EventSeverity.Info,
            $"Vehicle status changed: {vehicle.Name} {before} -> {after}",
            JsonSerializer.Serialize(new { before = new { status = before }, after = new { status = after }, reason = request.Reason }),
            "FleetAssets.Api",
            actor ?? "system"
        ), ct);

        await _vehicles.SaveChangesAsync(ct);
        await _events.SaveChangesAsync(ct);

        return ToDto(vehicle);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid correlationId, string? actor, CancellationToken ct)
    {
        var vehicle = await _vehicles.GetByIdAsync(id, ct);
        if (vehicle is null) return false;

        await _vehicles.DeleteAsync(vehicle, ct);

        await _events.AddAsync(new EventLog(
            EventType.VehicleDeleted,
            "Vehicle",
            vehicle.Id,
            correlationId,
            EventSeverity.Warning,
            $"Vehicle deleted: {vehicle.Name} ({vehicle.Type})",
            JsonSerializer.Serialize(new { before = new { vehicle.Id, vehicle.Name, vehicle.Type, Status = vehicle.Status.ToString(), vehicle.LastKnownLocation } }),
            "FleetAssets.Api",
            actor ?? "system"
        ), ct);

        await _vehicles.SaveChangesAsync(ct);
        await _events.SaveChangesAsync(ct);

        return true;
    }

    private static VehicleDto ToDto(Vehicle v) =>
        new(
            v.Id,
            v.Name,
            v.Type,
            v.Status.ToString(),
            v.LastKnownLocation,
            v.CreatedAtUtc,
            v.UpdatedAtUtc
        );
}
