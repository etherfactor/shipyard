using EtherGizmos.Common.Abstractions;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000092;

[CreatedAt(year: 2026, month: 06, day: 06, hour: 09, minute: 00, description: "Create notification meta tables", trackingId: 92)]
public class Migration001_AddNotificationMetaTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("notification_channels")
            .WithColumn("notification_channel_id").AsString(100).PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("config_schema").AsString(int.MaxValue).NotNullable();

        Create.Table("notification_schedules")
            .WithColumn("notification_schedule_id").AsString(100).PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("config_schema").AsString(int.MaxValue).NotNullable();

        Create.Table("notification_events")
            .WithColumn("notification_event_id").AsString(200).PrimaryKey()
            .WithColumn("name").AsString(200).NotNullable();

        Create.Table("notification_event_channel_schedules")
            .WithColumn("notification_event_id").AsString(200).PrimaryKey()
            .WithColumn("notification_channel_id").AsString(100).PrimaryKey()
            .WithColumn("notification_schedule_id").AsString(100).PrimaryKey();

        Create.ForeignKey("FK_notification_event_channel_schedules_notification_event_id")
            .FromTable("notification_event_channel_schedules").ForeignColumn("notification_event_id")
            .ToTable("notification_events").PrimaryColumn("notification_event_id");

        Create.ForeignKey("FK_notification_event_channel_schedules_notification_channel_id")
            .FromTable("notification_event_channel_schedules").ForeignColumn("notification_channel_id")
            .ToTable("notification_channels").PrimaryColumn("notification_channel_id");

        Create.ForeignKey("FK_notification_event_channel_schedules_notification_schedule_id")
            .FromTable("notification_event_channel_schedules").ForeignColumn("notification_schedule_id")
            .ToTable("notification_schedules").PrimaryColumn("notification_schedule_id");
    }
}
