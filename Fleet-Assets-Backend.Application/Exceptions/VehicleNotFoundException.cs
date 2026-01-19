namespace Fleet_Assets_Backend.Application.Exceptions;

public sealed class VehicleNotFoundException(Guid vehicleId) : Exception("Vehicle not found.")
{
    public Guid VehicleId { get; } = vehicleId;
}
