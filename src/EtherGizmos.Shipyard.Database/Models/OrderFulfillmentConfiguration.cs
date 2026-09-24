using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class OrderFulfillmentConfiguration : IEntityTypeConfiguration<OrderFulfillment>
{
    public void Configure(
        EntityTypeBuilder<OrderFulfillment> entity)
    {
        entity.ToTable("order_fulfillments", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("order_fulfillment_id");

        entity.Property(e => e.OrderId)
            .HasColumnName("order_id");

        entity.HasOne(e => e.Order)
            .WithMany(e => e.OrderFulfillments)
            .HasForeignKey(e => e.OrderId);

        entity.Property(e => e.Sender)
            .HasColumnName("sender");

        entity.Property(e => e.OrderFulfillmentStatusType)
            .HasColumnName("order_fulfillment_status_type_id");

        entity.Property(e => e.ShippedOn)
            .HasColumnName("shipped_on");

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

        entity.HasMany(e => e.OrderFulfillmentItems)
            .WithOne(e => e.OrderFulfillment)
            .HasForeignKey(e => e.OrderFulfillmentId);
    }
}
