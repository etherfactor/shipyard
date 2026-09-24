using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class TrackingStatusTypeConfiguration : IEntityTypeConfiguration<TrackingStatusType>
{
    public void Configure(
        EntityTypeBuilder<TrackingStatusType> entity)
    {
        entity.ToTable("tracking_status_types", table => table.HasTrigger("TR"));

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("tracking_status_type_id");

        entity.AuditProperties();

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.PollingFactor)
            .HasColumnName("polling_factor");

        entity.Property(e => e.IsFinal)
            .HasColumnName("is_final");
    }
}
