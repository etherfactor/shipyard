using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000000;

[CreatedAt(year: 2025, month: 08, day: 20, hour: 18, minute: 00, description: "Create user tables")]
public class Migration005_CreateUserTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[users]
         */
        Create.Table("users")
            .WithColumn("user_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithAuditColumns()
            .WithColumn("username").AsString(50).NotNullable()
            .WithColumn("email_address").AsString(320).Nullable()
            .WithColumn("password_hash").AsString(int.MaxValue).NotNullable()
            .WithColumn("given_name").AsString(70).Nullable()
            .WithColumn("family_name").AsString(70).Nullable()
            .WithColumn("full_name").AsString(150).Nullable();

        Create.Index("IX_users_username")
            .OnTable("users")
            .OnColumn("username")
            .Ascending();

        Create.Index("IX_users_email_address")
            .OnTable("users")
            .OnColumn("email_address")
            .Ascending();
    }
}
