namespace Fleet_Assets_Backend.Application.Dtos.Vehicle;

public sealed record ChangeVehicleStatusRequest(
    string Status,
    string? Reason = null
);
