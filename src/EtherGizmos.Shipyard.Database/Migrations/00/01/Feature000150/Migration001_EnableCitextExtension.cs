using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;
using Npgsql;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000150;

[CreatedAt(year: 2025, month: 01, day: 01, hour: 00, minute: 00, description: "Enable citext extension", trackingId: 150)]
public class Migration001_EnableCitextExtension : MigrationExtension
{
    public override void Up()
    {
        IfDatabase(ProcessorIdConstants.PostgreSQL)
            .Execute.Sql("""
                create extension if not exists citext;
                """);

        Execute.WithConnection((conn, tran) =>
        {
            if (conn is NpgsqlConnection npgsql)
            {
                npgsql.ReloadTypes();
            }
        });
    }

    public override void Down()
    {
    }
}
