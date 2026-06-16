using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AppNotificationEventConfiguration : IEntityTypeConfiguration<AppNotificationEvent>
{
    public void Configure(
        EntityTypeBuilder<AppNotificationEvent> entity)
    {
        entity.ToTable("notification_events");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("notification_event_id");

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.HasMany(e => e.Supports)
            .WithOne(e => e.NotificationEvent)
            .HasForeignKey(e => e.NotificationEventId);
    }
}
