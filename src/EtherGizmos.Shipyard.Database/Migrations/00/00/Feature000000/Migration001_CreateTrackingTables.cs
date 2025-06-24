using EtherGizmos.Shipyard.Database.Extensions;
using EtherGizmos.Shipyard.Database.Migrations.Core;
using System.Data;

namespace EtherGizmos.Shipyard.Database.Migrations._00._00.Feature000000;

[CreatedAt(year: 2025, month: 06, day: 18, hour: 18, minute: 00, description: "Create tracking tables")]
public class Migration001_CreateTrackingTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[status_types]
         */
        Create.Table("status_types")
            .WithColumn("status_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("polling_factor").AsDecimal(12, 3).NotNullable()
            .WithColumn("is_final").AsBoolean().NotNullable();

        Create.AuditTriggerV1("status_types", ("status_type_id", DbType.Int32));

        /*
         * Create [dbo].[step_types]
         */
        Create.Table("step_types")
            .WithColumn("step_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.AuditTriggerV1("step_types", ("step_type_id", DbType.Int32));

        /*
         * Create [dbo].[carriers]
         */
        Create.Table("carriers")
            .WithColumn("carrier_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("slug").AsAnsiString(20).NotNullable();

        Create.AuditTriggerV1("carriers", ("carrier_id", DbType.Int32));

        /*
         * Create [dbo].[carrier_status_rules]
         */
        Create.Table("carrier_status_rules")
            .WithColumn("carrier_status_rule_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("carrier_id").AsInt32().NotNullable()
            .WithColumn("pattern").AsString(int.MaxValue).NotNullable()
            .WithColumn("status_type_id").AsInt32().NotNullable()
            .WithColumn("priority").AsInt32().NotNullable()
            .WithColumn("is_active").AsBoolean().NotNullable();

        Create.AuditTriggerV1("carrier_status_rules", ("carrier_status_rule_id", DbType.Int32));

        Create.ForeignKey("FK_carrier_status_rules_carrier_id")
            .FromTable("carrier_status_rules").ForeignColumn("carrier_id")
            .ToTable("carriers").PrimaryColumn("carrier_id");

        Create.ForeignKey("FK_carrier_status_rules_status_type_id")
            .FromTable("carrier_status_rules").ForeignColumn("status_type_id")
            .ToTable("status_types").PrimaryColumn("status_type_id");

        Create.Index("IX_carrier_status_rules_carrier_id_status_type_id")
            .OnTable("carrier_status_rules")
            .OnColumn("carrier_id")
            .Ascending()
            .OnColumn("status_type_id")
            .Ascending();

        /*
         * Create [dbo].[carrier_runbook_steps]
         */
        Create.Table("carrier_runbook_steps")
            .WithColumn("carrier_runbook_step_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("carrier_id").AsInt32().NotNullable()
            .WithColumn("step_type_id").AsInt32().NotNullable()
            .WithColumn("payload").AsString(int.MaxValue).NotNullable();

        Create.AuditTriggerV1("carrier_runbook_steps", ("carrier_runbook_step_id", DbType.Int32));

        Create.ForeignKey("FK_carrier_runbook_steps_carrier_id")
            .FromTable("carrier_runbook_steps").ForeignColumn("carrier_id")
            .ToTable("carriers").PrimaryColumn("carrier_id");

        Create.ForeignKey("FK_carrier_runbook_steps_step_type_id")
            .FromTable("carrier_runbook_steps").ForeignColumn("step_type_id")
            .ToTable("step_types").PrimaryColumn("step_type_id");

        Create.Index("IX_carrier_runbook_steps_carrier_id_step_type_id")
            .OnTable("carrier_runbook_steps")
            .OnColumn("carrier_id")
            .Ascending()
            .OnColumn("step_type_id")
            .Ascending();

        /*
         * Create [dbo].[packages]
         */
        Create.Table("packages")
            .WithColumn("package_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("carrier_id").AsInt32().NotNullable()
            .WithColumn("tracking_number").AsAnsiString(200).NotNullable()
            .WithColumn("contents").AsString(int.MaxValue).Nullable()
            .WithColumn("estimated_delivery_at_utc").AsDateTime2().Nullable()
            .WithColumn("last_poll_at_utc").AsDateTime2().NotNullable()
            .WithColumn("next_poll_at_utc").AsDateTime2().NotNullable()
            .WithColumn("last_status_type_id").AsInt32().NotNullable()
            .WithColumn("is_delivered").AsBoolean().NotNullable();

        Create.AuditTriggerV1("packages", ("package_id", DbType.Int32));

        Create.ForeignKey("FK_packages_carrier_id")
            .FromTable("packages").ForeignColumn("carrier_id")
            .ToTable("carriers").PrimaryColumn("carrier_id");

        Create.Index("IX_packages_carrier_id")
            .OnTable("packages")
            .OnColumn("carrier_id");

        /*
         * Create [dbo].[tracking_updates]
         */
        Create.Table("tracking_updates")
            .WithColumn("tracking_update_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("package_id").AsInt32().NotNullable()
            .WithColumn("occurred_at_utc").AsDateTime2().NotNullable()
            .WithColumn("status_type_id").AsInt32().NotNullable()
            .WithColumn("location").AsString(200).Nullable()
            .WithColumn("description").AsString(200).Nullable();

        Create.AuditTriggerV1("tracking_updates", ("tracking_update_id", DbType.Int32));
    }

    public override void Down()
    {
        /*
         * Delete [dbo].[tracking_updates]
         */
        Delete.Table("tracking_updates");

        /*
         * Delete [dbo].[packages]
         */
        Delete.Table("packages");

        /*
         * Delete [dbo].[carrier_status_types]
         */
        Delete.Table("carrier_status_types");

        /*
         * Delete [dbo].[carriers]
         */
        Delete.Table("carriers");

        /*
         * Delete [dbo].[status_types]
         */
        Delete.Table("status_types");
    }
}
