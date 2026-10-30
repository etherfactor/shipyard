using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000165;

[CreatedAt(year: 2026, month: 10, day: 29, hour: 14, minute: 00, description: "Load security tables for orders", trackingId: 165)]
public class Migration004_LoadSecurityTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [acl].[securable_types]
         */
        Merge.IntoTable("securable_types").InSchema("acl")
            .Row(new { securable_type_id = 30, name = "Order", description = "An order being tracked." })
            .Match(e => new { e.securable_type_id });
    }

    public override void Down()
    {
    }
}
