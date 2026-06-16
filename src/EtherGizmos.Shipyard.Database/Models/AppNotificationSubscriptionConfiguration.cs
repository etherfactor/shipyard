using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AppNotificationSubscriptionConfiguration : IEntityTypeConfiguration<AppNotificationSubscription>
{
    public void Configure(
        EntityTypeBuilder<AppNotificationSubscription> entity)
    {
        entity.ToTable("notification_subscriptions");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("notification_subscription_id");

        entity.Property(e => e.UserId)
            .HasColumnName("user_id");

        entity.Property(e => e.EventType)
            .HasColumnName("event_type");

        entity.HasOne(e => e.Event)
            .WithMany()
            .HasForeignKey(e => e.EventType);

        entity.Property(e => e.ChannelKey)
            .HasColumnName("channel_key");

        entity.HasOne(e => e.Channel)
            .WithMany()
            .HasForeignKey(e => e.ChannelKey);

        entity.Property(e => e.ChannelConfigRaw)
            .HasColumnName("channel_config");

        entity.Property(e => e.ScheduleType)
            .HasColumnName("schedule_type");

        entity.HasOne(e => e.Schedule)
            .WithMany()
            .HasForeignKey(e => e.ScheduleType);

        entity.Property(e => e.ScheduleConfigRaw)
            .HasColumnName("schedule_config");

        entity.Property(e => e.IsEnabled)
            .HasColumnName("is_enabled");

        entity.Property(e => e.LastNotificationAt)
            .HasColumnName("last_notification_at");

        entity.Property(e => e.NextNotificationAt)
            .HasColumnName("next_notification_at");
    }
}
