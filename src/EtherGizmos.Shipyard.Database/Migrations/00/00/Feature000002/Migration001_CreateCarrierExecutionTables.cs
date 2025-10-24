using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Migrations.Core;
using System.Data;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000002;

[CreatedAt(year: 2025, month: 10, day: 21, hour: 18, minute: 00, description: "Create carrier execution tables")]
public class Migration001_CreateCarrierExecutionTables : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [dbo].[execution_status_types]
         */
        Create.Table("execution_status_types")
            .WithColumn("execution_status_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.AuditTriggerV1("execution_status_types", ("execution_status_type_id", DbType.Int32));

        /*
         * Create [dbo].[carrier_executions]
         */
        Create.Table("carrier_executions")
            .WithColumn("carrier_execution_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("carrier_id").AsInt32().NotNullable()
            .WithColumn("started_at_utc").AsDateTime2().Nullable()
            .WithColumn("completed_at_utc").AsDateTime2().Nullable()
            .WithColumn("execution_status_type_id").AsInt32().NotNullable()
            .WithColumn("step_count").AsInt16().NotNullable()
            .WithColumn("failure_step_index").AsInt16().Nullable();

        Create.AuditTriggerV1("carrier_executions", ("carrier_execution_id", DbType.Int32));

        Create.ForeignKey("FK_carrier_executions_carrier_id")
            .FromTable("carrier_executions").ForeignColumn("carrier_id")
            .ToTable("carriers").PrimaryColumn("carrier_id");

        Create.ForeignKey("FK_carrier_executions_execution_status_type_id")
            .FromTable("carrier_executions").ForeignColumn("execution_status_type_id")
            .ToTable("execution_status_types").PrimaryColumn("execution_status_type_id");

        Create.Index("IX_carrier_executions_carrier_id")
            .OnTable("carrier_executions")
            .OnColumn("carrier_id")
            .Ascending();

        /*
         * Create [dbo].[carrier_execution_artifacts]
         */
        Create.Table("carrier_execution_artifacts")
            .WithColumn("carrier_execution_artifact_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("carrier_execution_id").AsInt32().NotNullable()
            .WithColumn("artifact_uri").AsString(300).NotNullable()
            .WithColumn("content_type").AsString(50).NotNullable()
            .WithColumn("bytes").AsInt64().NotNullable()
            .WithColumn("step_index").AsInt16().Nullable();

        Create.AuditTriggerV1("carrier_execution_artifacts", ("carrier_execution_artifact_id", DbType.Int32));

        Create.ForeignKey("FK_carrier_execution_artifacts_carrier_execution_id")
            .FromTable("carrier_execution_artifacts").ForeignColumn("carrier_execution_id")
            .ToTable("carrier_executions").PrimaryColumn("carrier_execution_id");

        Create.Index("IX_carrier_execution_artifacts_carrier_execution_id")
            .OnTable("carrier_execution_artifacts")
            .OnColumn("carrier_execution_id")
            .Ascending();
    }

    public override void Down()
    {
        Delete.Table("carrier_execution_artifacts");

        Delete.Table("carrier_executions");

        Delete.Table("execution_status_types");
    }
}
