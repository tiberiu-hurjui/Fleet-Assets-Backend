using Fleet_Assets_Backend.Application.Dtos.EventLog;

namespace Fleet_Assets_Backend.Application.Interfaces;

public interface IEventQueryService
{
    Task<List<EventLogDto>> GetAsync(GetEventsQuery query, CancellationToken ct);
    Task<EventLogDto?> GetByIdAsync(Guid id, CancellationToken ct);
}
