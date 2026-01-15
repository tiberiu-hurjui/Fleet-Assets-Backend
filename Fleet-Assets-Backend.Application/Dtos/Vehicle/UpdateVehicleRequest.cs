namespace Fleet_Assets_Backend.Application.Dtos.Vehicle;

public sealed record UpdateVehicleRequest(
    string Name,
    string Type,
    string? LastKnownLocation
);
