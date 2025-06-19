using EtherGizmos.Shipyard.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Database.Models;

public class TrackingUpdateConfiguration : IEntityTypeConfiguration<TrackingUpdate>
{
    public void Configure(
        EntityTypeBuilder<TrackingUpdate> entity)
    {
        entity.ToTable("tracking_updates", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("tracking_update_id");

        entity.Property(e => e.PackageId)
            .HasColumnName("package_id");

        entity.HasOne(e => e.Package)
            .WithMany()
            .HasForeignKey(e => e.PackageId);

        entity.Property(e => e.OccurredAt)
            .HasColumnName("occurred_at_utc");

        entity.Property(e => e.StatusTypeId)
            .HasColumnName("status_type_id");

        entity.HasOne(e => e.StatusType)
            .WithMany()
            .HasForeignKey(e => e.StatusTypeId);

        entity.Property(e => e.Location)
            .HasColumnName("location");

        entity.Property(e => e.Description)
            .HasColumnName("description");
    }
}
