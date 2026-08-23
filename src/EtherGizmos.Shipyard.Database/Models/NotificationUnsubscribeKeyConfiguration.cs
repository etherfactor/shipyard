using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text;

namespace EtherGizmos.Shipyard.Models;

public class NotificationUnsubscribeKeyConfiguration : IEntityTypeConfiguration<NotificationUnsubscribeKey>
{
    public void Configure(
        EntityTypeBuilder<NotificationUnsubscribeKey> entity)
    {
        entity.ToTable("unsubscribe_keys", schema: "notification");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("unsubscribe_key_id");

        entity.Property(e => e.SubscriptionId)
            .HasColumnName("subscription_id");

        entity.Property(e => e.Value)
            .HasColumnName("value")
            .HasConversion(
                app => Encoding.UTF8.GetBytes(app),
                db => Encoding.UTF8.GetString(db));
    }
}
