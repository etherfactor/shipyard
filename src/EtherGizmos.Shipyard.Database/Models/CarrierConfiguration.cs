using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class CarrierConfiguration : IEntityTypeConfiguration<Carrier>
{
    public void Configure(
        EntityTypeBuilder<Carrier> entity)
    {
        entity.ToTable("carriers", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("carrier_id");

        entity.AuditProperties();

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.Slug)
            .HasColumnName("slug");

        entity.Property(e => e.SecurableId)
            .HasColumnName("securable_id");

        entity.HasOne(e => e.Securable);
    }
}
