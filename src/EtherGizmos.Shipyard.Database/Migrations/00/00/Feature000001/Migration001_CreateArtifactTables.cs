using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000001;

[CreatedAt(year: 2025, month: 10, day: 19, hour: 15, minute: 00, description: "Create artifact tables")]
public class Migration001_CreateArtifactTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[artifacts]
         */
        Create.Table("artifacts")
            .WithColumn("artifact_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("uri").AsString(200).NotNullable()
            .WithColumn("content_type").AsString(100).NotNullable()
            .WithColumn("file_name").AsString(100).NotNullable()
            .WithColumn("bytes").AsInt64().NotNullable()
            .WithColumn("physical_path").AsString(255).NotNullable();

        Create.AuditTriggerV1("artifacts", ("artifact_id", DbType.Guid));

        Create.Index("IX_artifacts_uri")
            .OnTable("artifacts")
            .OnColumn("uri")
            .Unique();
    }

    public override void Down()
    {
        Delete.Table("artifacts");
    }
}
