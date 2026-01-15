using Fleet_Assets_Backend.Domain.Entities;

namespace Fleet_Assets_Backend.Infrastructure.Interfaces;

public interface IEventLogRepository
{
    Task AddAsync(EventLog log, CancellationToken ct);
    Task<EventLog?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<List<EventLog>> QueryAsync(
        string? entityType,
        Guid? entityId,
        string? eventType,
        DateTime? fromUtc,
        DateTime? toUtc,
        int take,
        CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}