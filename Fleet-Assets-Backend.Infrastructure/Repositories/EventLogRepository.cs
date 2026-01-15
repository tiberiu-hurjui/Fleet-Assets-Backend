using Fleet_Assets_Backend.Domain.Entities;
using Fleet_Assets_Backend.Infrastructure.Interfaces;
using Fleet_Assets_Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fleet_Assets_Backend.Infrastructure.Repositories;

public class EventLogRepository(FleetAssetsDbContext db) : IEventLogRepository
{
    private readonly FleetAssetsDbContext _db = db;

    public async Task AddAsync(EventLog log, CancellationToken ct)
    {
        await _db.EventLogs.AddAsync(log, ct);
    }

    public async Task<EventLog?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.EventLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<List<EventLog>> QueryAsync(
        string? entityType,
        Guid? entityId,
        string? eventType,
        DateTime? fromUtc,
        DateTime? toUtc,
        int take,
        CancellationToken ct)
    {
        take = Math.Clamp(take, 1, 500);

        var q = _db.EventLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            var et = entityType.Trim();
            q = q.Where(e => e.EntityType == et);
        }

        if (entityId.HasValue)
        {
            var id = entityId.Value;
            q = q.Where(e => e.EntityId == id);
        }

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            var evt = eventType.Trim();
            q = q.Where(e => e.EventType.ToString() == evt);
        }

        if (fromUtc.HasValue)
        {
            var from = DateTime.SpecifyKind(fromUtc.Value, DateTimeKind.Utc);
            q = q.Where(e => e.OccurredAtUtc >= from);
        }

        if (toUtc.HasValue)
        {
            var to = DateTime.SpecifyKind(toUtc.Value, DateTimeKind.Utc);
            q = q.Where(e => e.OccurredAtUtc <= to);
        }

        return await q
            .OrderByDescending(e => e.OccurredAtUtc)
            .Take(take)
            .ToListAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
