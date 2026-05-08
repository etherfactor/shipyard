using EtherGizmos.Common.Abstractions;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000153;

[CreatedAt(year: 2026, month: 05, day: 05, hour: 18, minute: 00, description: "Add system columns to users", trackingId: 153)]
public class Migration001_AddSystemColumnsToUsers : AutoReversingMigration
{
    public override void Up()
    {
        //Alter.Table("users")
        //    .AddColumn("system_id").AsGuid().Nullable()
        //    .AddColumn("is_system_managed").AsBoolean().NotNullable().WithDefaultValue(false)
        //    .AddColumn("is_interactive_login_enabled").AsBoolean().NotNullable().WithDefaultValue(true);

        //Create.Index("IX_users_system_id")
        //    .OnTable("users")
        //    .OnColumn("system_id").Ascending();
    }
}
