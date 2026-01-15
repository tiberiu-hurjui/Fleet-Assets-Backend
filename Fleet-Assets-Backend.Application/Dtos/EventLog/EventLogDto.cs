namespace Fleet_Assets_Backend.Application.Dtos.EventLog;

public sealed record EventLogDto(
  Guid Id,
  DateTime OccurredAtUtc,
  string EventType,
  string EntityType,
  Guid EntityId,
  Guid CorrelationId,
  string Severity,
  string Summary,
  string DataJson,
  string Source,
  string? Actor
);
