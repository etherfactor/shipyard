using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000002;

[CreatedAt(year: 2025, month: 10, day: 21, hour: 18, minute: 30, description: "Load carrier execution tables")]
public class Migration002_LoadCarrierExecutionTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [dbo].[execution_status_types]
         */
        Merge.IntoTable("execution_status_types")
            .Row(new { execution_status_type_id = 1, name = "Queued", description = "The execution has been queued." })
            .Row(new { execution_status_type_id = 10, name = "Running", description = "The execution is actively running." })
            .Row(new { execution_status_type_id = 100, name = "Successful", description = "The execution has completed successfully." })
            .Row(new { execution_status_type_id = -100, name = "Failed", description = "The execution has failed during execution." })
            .Row(new { execution_status_type_id = -10, name = "Timed out", description = "The execution failed to complete on time and was considered abandoned." })
            .Row(new { execution_status_type_id = -20, name = "Cancelled", description = "The execution was manually cancelled." })
            .Match(e => new { e.execution_status_type_id });
    }

    public override void Down()
    {
    }
}
