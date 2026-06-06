using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class NotificationEventChannelScheduleConfiguration : IEntityTypeConfiguration<NotificationEventChannelSchedule>
{
    public void Configure(
        EntityTypeBuilder<NotificationEventChannelSchedule> entity)
    {
        entity.ToTable("notification_channels");

        entity.HasKey(e => new { e.NotificationEventId, e.NotificationChannelId, e.NotificationScheduleId });

        entity.Property(e => e.NotificationEventId)
            .HasColumnName("notification_event_id");

        entity.HasOne(e => e.NotificationEvent)
            .WithMany(e => e.Supports)
            .HasForeignKey(e => e.NotificationEventId);

        entity.Property(e => e.NotificationChannelId)
            .HasColumnName("notification_channel_id");

        entity.HasOne(e => e.NotificationChannel)
            .WithMany()
            .HasForeignKey(e => e.NotificationChannelId);

        entity.Property(e => e.NotificationScheduleId)
            .HasColumnName("notification_schedule_id");

        entity.HasOne(e => e.NotificationSchedule)
            .WithMany()
            .HasForeignKey(e => e.NotificationScheduleId);
    }
}
