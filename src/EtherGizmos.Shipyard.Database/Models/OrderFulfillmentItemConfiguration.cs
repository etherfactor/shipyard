using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class OrderFulfillmentItemConfiguration : IEntityTypeConfiguration<OrderFulfillmentItem>
{
    public void Configure(
        EntityTypeBuilder<OrderFulfillmentItem> entity)
    {
        entity.ToTable("order_fulfillment_items", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("order_fulfillment_item_id");

        entity.Property(e => e.OrderFulfillmentId)
            .HasColumnName("order_fulfillment_id");

        entity.HasOne(e => e.OrderFulfillment)
            .WithMany(e => e.OrderFulfillmentItems)
            .HasForeignKey(e => e.OrderFulfillmentId);

        entity.Property(e => e.OrderItemId)
            .HasColumnName("order_item_id");

        entity.HasOne(e => e.OrderItem)
            .WithMany()
            .HasForeignKey(e => e.OrderItemId);

        entity.Property(e => e.ExpectedQuantity)
            .HasColumnName("expected_quantity");

        entity.Property(e => e.ReceivedQuantity)
            .HasColumnName("received_quantity");
    }
}
