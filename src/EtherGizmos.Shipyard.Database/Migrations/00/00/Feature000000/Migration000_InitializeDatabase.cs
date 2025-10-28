using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000000;

[CreatedAt(year: 2025, month: 06, day: 17, hour: 18, minute: 00, description: "Initialize database")]
public class Migration000_InitializeDatabase : MigrationExtension
{
    public override void Up()
    {
        IfDatabase(ProcessorIdConstants.PostgreSQL)
            .Execute.Sql("""
            create extension if not exists "uuid-ossp";
            """);
    }

    public override void Down()
    {
    }
}
