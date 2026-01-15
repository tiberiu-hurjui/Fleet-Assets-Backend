using Fleet_Assets_Backend.Domain.Enums;

namespace Fleet_Assets_Backend.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = default!;
    public string Type { get; private set; } = default!;
    public VehicleStatus Status { get; private set; } = VehicleStatus.Active;

    public string? LastKnownLocation { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; private set; } = DateTime.UtcNow;

    public byte[] RowVersion { get; private set; } = [];

    private Vehicle() { }

    public Vehicle(string name, string type, string? lastKnownLocation = null)
    {
        Name = name.Trim();
        Type = type.Trim();
        LastKnownLocation = lastKnownLocation?.Trim();
        Touch();
    }

    public void UpdateDetails(string name, string type, string? lastKnownLocation)
    {
        Name = name.Trim();
        Type = type.Trim();
        LastKnownLocation = lastKnownLocation?.Trim();
        Touch();
    }

    public void ChangeStatus(VehicleStatus newStatus)
    {
        if (Status == newStatus) return;
        Status = newStatus;
        Touch();
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}
