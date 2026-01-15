namespace Fleet_Assets_Backend.Application.Dtos.Vehicle;

public sealed record VehicleDto(
    Guid Id,
    string Name,
    string Type,
    string Status,
    string? LastKnownLocation,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);
