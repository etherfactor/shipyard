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

        entity.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId);

        entity.Property(e => e.CarrierId)
            .HasColumnName("carrier_id");

        entity.HasOne(e => e.Carrier)
            .WithMany()
            .HasForeignKey(e => e.CarrierId);

        entity.Property(e => e.TrackingNumber)
            .HasColumnName("tracking_number");

        entity.Property(e => e.IsItemized)
            .HasColumnName("is_itemized");

        entity.Property(e => e.Contents)
            .HasColumnName("contents");

        entity.Property(e => e.OrderFulfillmentId)
            .HasColumnName("order_fulfillment_id");

        entity.HasOne(e => e.OrderFulfillment)
            .WithOne()
            .HasForeignKey<Package>(e => e.OrderFulfillmentId);

        entity.Property(e => e.EstimatedDeliveryAt)
            .HasColumnName("estimated_delivery_at_utc");

        entity.Property(e => e.LastPollAt)
            .HasColumnName("last_poll_at_utc");

        entity.Property(e => e.NextPollAt)
            .HasColumnName("next_poll_at_utc");

        entity.Property(e => e.LastTrackingStatusTypeId)
            .HasColumnName("last_tracking_status_type_id");

        entity.HasOne(e => e.LastTrackingStatusType)
            .WithMany()
            .HasForeignKey(e => e.LastTrackingStatusTypeId);

        entity.Property(e => e.IsDelivered)
            .HasColumnName("is_delivered");

        entity.Property(e => e.ReceivedAt)
            .HasColumnName("received_at_utc");

        entity.Property(e => e.ReceivedByUserId)
            .HasColumnName("received_by_user_id");

        entity.HasOne(e => e.ReceivedByUser)
            .WithMany()
            .HasForeignKey(e => e.ReceivedByUserId);

        entity.Property(e => e.ResolvedAt)
            .HasColumnName("resolved_at_utc");

        entity.Property(e => e.ResolvedByUserId)
            .HasColumnName("resolved_by_user_id");

        entity.HasOne(e => e.ResolvedByUser)
            .WithMany()
            .HasForeignKey(e => e.ResolvedByUserId);

        entity.Property(e => e.ResolvedReason)
            .HasColumnName("resolved_reason");

        entity.HasMany(e => e.PackageItems)
            .WithOne(e => e.Package)
            .HasForeignKey(e => e.PackageId);

        entity.HasMany(e => e.TrackingUpdates)
            .WithOne(e => e.Package)
            .HasForeignKey(e => e.PackageId);

        entity.Property(e => e.GroupId)
            .HasColumnName("group_id");

        entity.HasOne(e => e.Group);

        entity.Property(e => e.SecurableId)
            .HasColumnName("securable_id");

        entity.HasOne(e => e.Securable);
    }
}
