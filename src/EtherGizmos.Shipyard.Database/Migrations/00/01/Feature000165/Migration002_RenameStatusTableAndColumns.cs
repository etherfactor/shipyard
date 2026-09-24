using EtherGizmos.Common.Abstractions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000165;

[CreatedAt(year: 2026, month: 09, day: 16, hour: 18, minute: 15, description: "Rename status table and columns", trackingId: 165)]
public class Migration002_RenameStatusTableAndColumns : Migration
{
    public override void Up()
    {
        /*
         * Rename [dbo].[status_types]
         */

        //Purge foreign keys
        Delete.ForeignKey("FK_carrier_status_rules_status_type_id")
            .OnTable("carrier_status_rules");

        Delete.AuditTriggerV1("status_types");

        Rename.Table("status_types")
            .To("tracking_status_types");

        Rename.Column("status_type_id")
            .OnTable("tracking_status_types")
            .To("tracking_status_type_id");

        Create.AuditTriggerV1("tracking_status_types", ("tracking_status_type_id", DbType.Int32));

        /*
         * Rename [dbo].[carrier_status_rules].[status_type_id]
         */
        Delete.Index("IX_carrier_status_rules_carrier_id_status_type_id")
            .OnTable("carrier_status_rules");

        Rename.Column("status_type_id")
            .OnTable("carrier_status_rules")
            .To("tracking_status_type_id");

        Create.ForeignKey("FK_carrier_status_rules_tracking_status_type_id")
            .FromTable("carrier_status_rules").ForeignColumn("tracking_status_type_id")
            .ToTable("tracking_status_types").PrimaryColumn("tracking_status_type_id");

        Create.Index("IX_carrier_status_rules_carrier_id_tracking_status_type_id")
            .OnTable("carrier_status_rules")
            .OnColumn("carrier_id")
            .Ascending()
            .OnColumn("tracking_status_type_id")
            .Ascending();

        /*
         * Rename [dbo].[packages].[last_status_type_id]
         */
        Rename.Column("last_status_type_id")
            .OnTable("packages")
            .To("last_tracking_status_type_id");

        Create.ForeignKey("FK_packages_last_tracking_status_type_id")
            .FromTable("packages").ForeignColumn("last_tracking_status_type_id")
            .ToTable("tracking_status_types").PrimaryColumn("tracking_status_type_id");

        /*
         * Rename [dbo].[tracking_updates].[status_type_id]
         */
        Rename.Column("status_type_id")
            .OnTable("tracking_updates")
            .To("tracking_status_type_id");

        Create.ForeignKey("FK_tracking_updates_tracking_status_type_id")
            .FromTable("tracking_updates").ForeignColumn("tracking_status_type_id")
            .ToTable("tracking_status_types").PrimaryColumn("tracking_status_type_id");
    }

    public override void Down()
    {
        /*
         * Rename [dbo].[status_types]
         */

        //Purge foreign keys
        Delete.ForeignKey("FK_carrier_status_rules_tracking_status_type_id")
            .OnTable("carrier_status_rules");

        Delete.ForeignKey("FK_packages_last_status_type_id")
            .OnTable("packages");

        Delete.ForeignKey("FK_tracking_updates_tracking_status_type_id")
            .OnTable("tracking_updates");

        Delete.AuditTriggerV1("tracking_status_types");

        Rename.Table("tracking_status_types")
            .To("status_types");

        Rename.Column("tracking_status_type_id")
            .OnTable("status_types")
            .To("status_type_id");

        Create.AuditTriggerV1("status_types", ("status_type_id", DbType.Int32));

        /*
         * Rename [dbo].[carrier_status_rules].[status_type_id]
         */
        Delete.Index("IX_carrier_status_rules_carrier_id_tracking_status_type_id")
            .OnTable("carrier_status_rules");

        Rename.Column("tracking_status_type_id")
            .OnTable("carrier_status_rules")
            .To("status_type_id");

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
         * Rename [dbo].[packages].[last_status_type_id]
         */
        Rename.Column("last_tracking_status_type_id")
            .OnTable("packages")
            .To("last_status_type_id");

        /*
         * Rename [dbo].[tracking_updates].[status_type_id]
         */
        Rename.Column("tracking_status_type_id")
            .OnTable("tracking_updates")
            .To("status_type_id");
    }
}
