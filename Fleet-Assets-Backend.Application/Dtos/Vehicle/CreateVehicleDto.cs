namespace Fleet_Assets_Backend.Application.Dtos.Vehicle;

public sealed record CreateVehicleRequest(
    string Name,
    string Type,
    string? LastKnownLocation
);
