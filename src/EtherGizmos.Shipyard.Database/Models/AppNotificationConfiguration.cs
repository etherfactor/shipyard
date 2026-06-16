using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class AppNotificationConfiguration : IEntityTypeConfiguration<AppNotification>
{
    public void Configure(
        EntityTypeBuilder<AppNotification> entity)
    {
        entity.ToTable("notifications");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.EventId)
            .HasColumnName("event_id");

        entity.Property(e => e.CreatedAt)
            .HasColumnName("created_at");

        entity.Property(e => e.SentAt)
            .HasColumnName("sent_at");

        entity.Property(e => e.Id)
            .HasColumnName("notification_id");

        entity.Property(e => e.NotificationSubscriptionId)
            .HasColumnName("notification_subscription_id");

        entity.HasOne(e => e.NotificationSubscription)
            .WithMany()
            .HasForeignKey(e => e.NotificationSubscriptionId);

        entity.Property(e => e.IsDerived)
            .HasColumnName("is_derived");

        entity.Property(e => e.PayloadType)
            .HasColumnName("payload_type");

        entity.Property(e => e.Payload)
            .HasColumnName("payload");

        entity.Property(e => e.Status)
            .HasColumnName("notification_status_type_id");

        entity.Property(e => e.Headers)
            .HasColumnName("headers");

        entity.Property(e => e.AttemptCount)
            .HasColumnName("attempt_count");

        entity.Property(e => e.LastAttemptAt)
            .HasColumnName("last_attempt_at_utc");

        entity.Property(e => e.LastError)
            .HasColumnName("last_error");

        entity.Property(e => e.LockId)
            .HasColumnName("lock_id");

        entity.Property(e => e.LockedBy)
            .HasColumnName("locked_by");

        entity.Property(e => e.LockedUntil)
            .HasColumnName("locked_until_utc");
    }
}
