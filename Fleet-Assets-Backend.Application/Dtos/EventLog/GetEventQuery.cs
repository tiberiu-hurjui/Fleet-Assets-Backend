namespace Fleet_Assets_Backend.Application.Dtos.EventLog;

public sealed record GetEventsQuery(
    string? EntityType = null,
    Guid? EntityId = null,
    string? EventType = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    int Take = 100
);
