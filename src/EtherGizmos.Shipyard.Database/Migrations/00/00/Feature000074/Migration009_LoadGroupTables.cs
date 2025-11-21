using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 15, hour: 18, minute: 30, description: "Load security tables for groups", trackingId: 74)]
public class Migration009_LoadGroupTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [acl].[securable_types]
         */
        Merge.IntoTable("securable_types").InSchema("acl")
            .Row(new { securable_type_id = 120, name = "Group", description = "A group of users for the purpose of record separation." })
            .Match(e => new { e.securable_type_id });
    }

    public override void Down()
    {
    }
}
