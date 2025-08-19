using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(
        EntityTypeBuilder<Package> entity)
    {
        entity.ToTable("packages", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        //entity.HasAlternateKey(e => new { e.CarrierId, e.TrackingNumber });

        entity.Property(e => e.Id)
            .HasColumnName("package_id");

        entity.AuditProperties();

        entity.Property(e => e.CarrierId)
            .HasColumnName("carrier_id");

        entity.HasOne(e => e.Carrier)
            .WithMany()
            .HasForeignKey(e => e.CarrierId);

        entity.Property(e => e.TrackingNumber)
            .HasColumnName("tracking_number");

        entity.Property(e => e.Contents)
            .HasColumnName("contents");

        entity.Property(e => e.EstimatedDeliveryAt)
            .HasColumnName("estimated_delivery_at_utc");

        entity.Property(e => e.LastPollAt)
            .HasColumnName("last_poll_at_utc");

        entity.Property(e => e.NextPollAt)
            .HasColumnName("next_poll_at_utc");

        entity.Property(e => e.LastStatusTypeId)
            .HasColumnName("last_status_type_id");

        entity.HasOne(e => e.LastStatusType)
            .WithMany()
            .HasForeignKey(e => e.LastStatusTypeId);

        entity.Property(e => e.IsDelivered)
            .HasColumnName("is_delivered");

        entity.HasMany(e => e.TrackingUpdates)
            .WithOne(e => e.Package)
            .HasForeignKey(e => e.PackageId);
    }
}
