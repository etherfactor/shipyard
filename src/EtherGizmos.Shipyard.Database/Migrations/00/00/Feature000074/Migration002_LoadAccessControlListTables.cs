using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 12, hour: 18, minute: 20, description: "Load access control list tables", trackingId: 74)]
public class Migration002_LoadAccessControlListTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [acl].[permissions]
         */
        Merge.IntoTable("permissions").InSchema("acl")
            .Row(new { permission_id = 1, name = "Read", description = "Enables the reading of records." })
            .Row(new { permission_id = 2, name = "Write", description = "Enables the creation and updating of records." })
            .Row(new { permission_id = 4, name = "Delete", description = "Enables the deletion of records." })
            .Match(e => new { e.permission_id });

        /*
         * Load [acl].[permission_grant_types]
         */
        Merge.IntoTable("permission_grant_types").InSchema("acl")
            .Row(new { permission_grant_type_id = -1, name = "Deny", description = "Denies all access." })
            .Row(new { permission_grant_type_id = 1, name = "Full", description = "Provides full access." })
            .Row(new { permission_grant_type_id = 2, name = "Filter", description = "Provides contextual filtered access." })
            .Match(e => new { e.permission_grant_type_id });

        /*
         * Load [acl].[principal_types]
         */
        Merge.IntoTable("principal_types").InSchema("acl")
            .Row(new { principal_type_id = 100, name = "User", description = "A user of the application." })
            .Row(new { principal_type_id = 110, name = "Role", description = "A collection of permissions that can be inherited." })
            .Match(e => new { e.principal_type_id });

        /*
         * Load [acl].[securable_types]
         */
        Merge.IntoTable("securable_types").InSchema("acl")
            .Row(new { securable_type_id = 10, name = "Carrier", description = "Carrier metadata defining how to track a package." })
            .Row(new { securable_type_id = 20, name = "Package", description = "A package being tracked." })
            .Row(new { securable_type_id = 100, name = "User", description = "A user of the application." })
            .Row(new { securable_type_id = 110, name = "Role", description = "A collection of permissions that can be inherited." })
            .Match(e => new { e.securable_type_id });
    }

    public override void Down()
    {
    }
}
