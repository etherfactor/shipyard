using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(
        EntityTypeBuilder<OrderItem> entity)
    {
        entity.ToTable("order_items", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("order_item_id");

        entity.Property(e => e.OrderId)
            .HasColumnName("order_id");

        entity.HasOne(e => e.Order)
            .WithMany(e => e.OrderItems)
            .HasForeignKey(e => e.OrderId);

        entity.Property(e => e.Sequence)
            .HasColumnName("sequence");

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.Quantity)
            .HasColumnName("quantity");

        entity.Property(e => e.QuantityUnits)
            .HasColumnName("quantity_units");

        entity.Property(e => e.UnitPrice)
            .HasColumnName("unit_price");

        entity.Property(e => e.TotalPrice)
            .HasColumnName("total_price");
    }
}
