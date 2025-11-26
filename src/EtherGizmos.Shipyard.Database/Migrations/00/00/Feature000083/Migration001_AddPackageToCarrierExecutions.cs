using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000083;

[CreatedAt(year: 2025, month: 11, day: 26, hour: 15, minute: 00, description: "Add package to carrier executions")]
public class Migration001_AddPackageToCarrierExecutions : AutoReversingMigration
{
    public override void Up()
    {
        Create.Column("package_id")
            .OnTable("carrier_executions")
            .AsInt32()
            .Nullable();

        Create.ForeignKey("FK_carrier_executions_package_id")
            .FromTable("carrier_executions").ForeignColumn("package_id")
            .ToTable("packages").PrimaryColumn("package_id");

        Create.Index("IX_carrier_executions_package_id")
            .OnTable("carrier_executions")
            .OnColumn("package_id")
            .Ascending();
    }
}
