using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000165;

[CreatedAt(year: 2026, month: 09, day: 16, hour: 18, minute: 30, description: "Load order tables", trackingId: 165)]
public class Migration003_LoadOrderTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [dbo].[order_fulfillment_status_types]
         */
        Merge.IntoTable("order_fulfillment_status_types")
            .Row(new { order_fulfillment_status_type_id = 1, name = "Pending", description = "The fulfillment has not yet shipped." })
            .Row(new { order_fulfillment_status_type_id = 10, name = "Shipped", description = "The fulfillment has been shipped." })
            .Row(new { order_fulfillment_status_type_id = 100, name = "Received", description = "The fulfillment has been received." })
            .Match(e => new { e.order_fulfillment_status_type_id });
    }

    public override void Down()
    {
    }
}
