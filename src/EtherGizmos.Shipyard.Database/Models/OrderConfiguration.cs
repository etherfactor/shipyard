using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(
        EntityTypeBuilder<Order> entity)
    {
        entity.ToTable("orders", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("order_id");

        entity.Property(e => e.Source)
            .HasColumnName("source");

        entity.Property(e => e.OrderNumber)
            .HasColumnName("order_number");

        entity.Property(e => e.OrderedOn)
            .HasColumnName("ordered_on");

        entity.Property(e => e.TotalPrice)
            .HasColumnName("total_price");

        entity.Property(e => e.OrderUrl)
            .HasColumnName("order_url");

        entity.Property(e => e.ExpectedShipStartOn)
            .HasColumnName("expected_ship_start_on");

        entity.Property(e => e.ExpectedShipEndOn)
            .HasColumnName("expected_ship_end_on");

        entity.HasMany(e => e.OrderItems)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId);
    }
}
