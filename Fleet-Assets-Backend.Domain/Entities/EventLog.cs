using Fleet_Assets_Backend.Domain.Enums;
using System;

namespace Fleet_Assets_Backend.Domain.Entities;

public class EventLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTime OccurredAtUtc { get; private set; } = DateTime.UtcNow;

    public EventType EventType { get; private set; }
    public string EntityType { get; private set; } = default!;
    public Guid EntityId { get; private set; }

    public Guid CorrelationId { get; private set; }

    public EventSeverity Severity { get; private set; } = EventSeverity.Info;

    public string Summary { get; private set; } = default!;
    public string DataJson { get; private set; } = "{}";

    public string Source { get; private set; } = "FleetAssets.Api";
    public string? Actor { get; private set; } = "system";

    private EventLog() { }

    public EventLog(
        EventType eventType,
        string entityType,
        Guid entityId,
        Guid correlationId,
        EventSeverity severity,
        string summary,
        string dataJson,
        string source = "FleetAssets.Api",
        string? actor = "system")
    {
        EventType = eventType;
        EntityType = entityType;
        EntityId = entityId;
        CorrelationId = correlationId;
        Severity = severity;
        Summary = summary;
        DataJson = string.IsNullOrWhiteSpace(dataJson) ? "{}" : dataJson;
        Source = source;
        Actor = actor;
    }
}
