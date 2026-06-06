using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class NotificationScheduleConfiguration : IEntityTypeConfiguration<NotificationSchedule>
{
    public void Configure(EntityTypeBuilder<NotificationSchedule> entity)
    {
        entity.ToTable("notification_schedules");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("notification_schedule_id");

        entity.Property(e => e.Name)
            .HasColumnName("name");

        entity.Property(e => e.ConfigSchema)
            .HasColumnName("config_schema");
    }
}
