using Fleet_Assets_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fleet_Assets_Backend.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable(nameof(Vehicle));

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Type).HasMaxLength(50).IsRequired();

        builder.Property(v => v.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(v => v.LastKnownLocation).HasMaxLength(200);

        builder.Property(v => v.CreatedAtUtc).IsRequired();
        builder.Property(v => v.UpdatedAtUtc).IsRequired();

        builder.Property(v => v.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(v => v.Status);
        builder.HasIndex(v => v.Type);
    }
}
