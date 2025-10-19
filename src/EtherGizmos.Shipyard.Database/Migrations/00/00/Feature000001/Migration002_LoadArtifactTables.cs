using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000001;

[CreatedAt(year: 2025, month: 10, day: 19, hour: 15, minute: 30, description: "Load artifact tables")]
public class Migration002_LoadArtifactTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Load [dbo].[artifact_types]
         */
        Merge.IntoTable("artifact_types")
            .Row(new { artifact_type_id = 1, name = "Text", description = "A raw text file." })
            .Row(new { artifact_type_id = 10, name = "WebP", description = "A lossy, compressed image file." })
            .Match(e => new { e.artifact_type_id });
    }

    public override void Down()
    {
    }
}
