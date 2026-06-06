using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
    public void Configure(
        EntityTypeBuilder<NotificationChannel> entity)
    {
        entity.ToTable("notification_channels");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("notification_channel_id");

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.ConfigSchema)
            .HasColumnName("config_schema");
    }
}
