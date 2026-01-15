using Fleet_Assets_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Fleet_Assets_Backend.Infrastructure.Persistence.Configurations;

public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
{
    public void Configure(EntityTypeBuilder<EventLog> builder)
    {
        builder.ToTable(nameof(EventLog));

        builder.HasKey(el => el.Id);

        builder.Property(el => el.OccurredAtUtc).IsRequired();

        builder.Property(el => el.EventType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(el => el.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(el => el.EntityId).IsRequired();

        builder.Property(el => el.CorrelationId).IsRequired();

        builder.Property(el => el.Severity)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(el => el.Summary).HasMaxLength(200).IsRequired();
        builder.Property(el => el.DataJson).IsRequired();

        builder.Property(el => el.Source).HasMaxLength(50).IsRequired();
        builder.Property(el => el.Actor).HasMaxLength(100);

        builder.HasIndex(el => el.OccurredAtUtc);
        builder.HasIndex(el => new { el.EntityType, el.EntityId, el.OccurredAtUtc });
        builder.HasIndex(el => el.CorrelationId);
        builder.HasIndex(el => new { el.EventType, el.OccurredAtUtc });
    }
}
