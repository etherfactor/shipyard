using EtherGizmos.Shipyard.Database.Migrations.Core;

namespace EtherGizmos.Shipyard.Database.Migrations._00._00.Feature000000;

[CreatedAt(year: 2025, month: 06, day: 18, hour: 18, minute: 30, description: "Load tracking tables")]
public class Migration002_LoadTrackingTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [dbo].[status_types]
         */
        Merge.IntoTable("status_types")
            .Row(new { status_type_id = 0, name = "Unknown", polling_factor = 4m, is_final = false })
            .Row(new { status_type_id = 1, name = "Waiting", polling_factor = 2m, is_final = false })
            .Row(new { status_type_id = 10, name = "In transit", polling_factor = 1m, is_final = false })
            .Row(new { status_type_id = 20, name = "Out for delivery", polling_factor = 0.167m, is_final = false })
            .Row(new { status_type_id = 100, name = "Delivered", polling_factor = 0m, is_final = true })
            .Row(new { status_type_id = -10, name = "Failed attempt", polling_factor = 1m, is_final = false })
            .Row(new { status_type_id = -100, name = "Returned", polling_factor = 0m, is_final = true })
            .Row(new { status_type_id = -200, name = "Expired", polling_factor = 0m, is_final = true })
            .Match(e => new { e.status_type_id });

        /*
         * Load [dbo].[step_types]
         */
        Merge.IntoTable("step_types")
            .Row(new { step_type_id = 1, name = "Navigate" })
            .Row(new { step_type_id = 10, name = "Wait for" })
            .Row(new { step_type_id = 20, name = "Click" })
            .Row(new { step_type_id = 30, name = "Extract" })
            .Row(new { step_type_id = 31, name = "Extract list" })
            .Row(new { step_type_id = 40, name = "Set" })
            .Row(new { step_type_id = 41, name = "Replace" })
            .Row(new { step_type_id = 100, name = "Return" })
            .Match(e => new { e.step_type_id });

        /*
         * Load [dbo].[carriers]
         */
        Merge.IntoTable("carriers")
            .Row(new { name = "USPS", slug = "usps" })
            .Match(e => new { e.slug });
    }

    public override void Down()
    {
    }
}
