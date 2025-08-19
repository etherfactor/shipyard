using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class TrackingUpdateConfiguration : IEntityTypeConfiguration<TrackingUpdate>
{
    public void Configure(
        EntityTypeBuilder<TrackingUpdate> entity)
    {
        entity.ToTable("tracking_updates", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("tracking_update_id");

        entity.AuditProperties();

        entity.Property(e => e.PackageId)
            .HasColumnName("package_id");

        entity.HasOne(e => e.Package)
            .WithMany(e => e.TrackingUpdates)
            .HasForeignKey(e => e.PackageId);

        entity.Property(e => e.OccurredAt)
            .HasColumnName("occurred_at_utc");

        entity.Property(e => e.StatusTypeId)
            .HasColumnName("status_type_id");

        entity.HasOne(e => e.StatusType)
            .WithMany()
            .HasForeignKey(e => e.StatusTypeId);

        entity.Property(e => e.Location)
            .HasColumnName("location")
            .HasMaxLength(200);

        entity.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(200);
    }
}
