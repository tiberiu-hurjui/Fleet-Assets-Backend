using Fleet_Assets_Backend.Application.Dtos.EventLog;
using Fleet_Assets_Backend.Application.Interfaces;
using Fleet_Assets_Backend.Infrasturcture.Interfaces;

namespace Fleet_Assets_Backend.Application.Events;

public class EventQueryService(IEventLogRepository events) : IEventQueryService
{
    private readonly IEventLogRepository _events = events;

    public async Task<List<EventLogDto>> GetAsync(GetEventsQuery query, CancellationToken ct)
    {
        var take = Math.Clamp(query.Take, 1, 500);

        var logs = await _events.QueryAsync(
            query.EntityType,
            query.EntityId,
            query.EventType,
            query.FromUtc,
            query.ToUtc,
            take,
            ct);

        return logs.Select(x => new EventLogDto(
            x.Id,
            x.OccurredAtUtc,
            x.EventType.ToString(),
            x.EntityType,
            x.EntityId,
            x.CorrelationId,
            x.Severity.ToString(),
            x.Summary,
            x.DataJson,
            x.Source,
            x.Actor
        )).ToList();
    }

    public async Task<EventLogDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var x = await _events.GetByIdAsync(id, ct);
        if (x is null) return null;

        return new EventLogDto(
            x.Id,
            x.OccurredAtUtc,
            x.EventType.ToString(),
            x.EntityType,
            x.EntityId,
            x.CorrelationId,
            x.Severity.ToString(),
            x.Summary,
            x.DataJson,
            x.Source,
            x.Actor
        );
    }
}
