using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class PackageItemConfiguration : IEntityTypeConfiguration<PackageItem>
{
    public void Configure(
        EntityTypeBuilder<PackageItem> entity)
    {
        entity.ToTable("package_items", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("package_item_id");

        entity.Property(e => e.PackageId)
            .HasColumnName("package_id");

        entity.HasOne(e => e.Package)
            .WithMany(e => e.PackageItems)
            .HasForeignKey(e => e.PackageId);

        entity.Property(e => e.Sequence)
            .HasColumnName("sequence");

        entity.Property(e => e.OrderFulfillmentItemId)
            .HasColumnName("order_fulfillment_item_id");

        entity.HasOne(e => e.OrderFulfillmentItem)
            .WithOne()
            .HasForeignKey<PackageItem>(e => e.OrderFulfillmentItemId);

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.Quantity)
            .HasColumnName("quantity");

        entity.Property(e => e.QuantityUnits)
            .HasColumnName("quantity_units");
    }
}
