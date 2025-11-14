using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 12, hour: 18, minute: 40, description: "Create role tables", trackingId: 74)]
public class Migration003_CreateRoleTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[roles]
         */
        Create.Table("roles")
            .WithColumn("role_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("system_id").AsGuid().Nullable()
            .WithColumn("principal_id").AsGuid().NotNullable()
            .WithColumn("securable_id").AsGuid().NotNullable();

        Create.ForeignKey("FK_roles_principal_id")
            .FromTable("roles").ForeignColumn("principal_id")
            .ToTable("principals").InSchema("acl").PrimaryColumn("principal_id");

        /*
         * Create [dbo].[roles]
         */
        Create.Table("role_users")
            .WithColumn("role_id").AsInt32().PrimaryKey()
            .WithColumn("user_id").AsGuid().PrimaryKey()
            .WithAuditColumns();

        Create.Index("IX_role_users_user_id_role_id")
            .OnTable("role_users")
            .OnColumn("user_id")
            .Ascending()
            .OnColumn("role_id")
            .Ascending();

        Create.ForeignKey("FK_role_users_role_id")
            .FromTable("role_users").ForeignColumn("role_id")
            .ToTable("roles").PrimaryColumn("role_id");

        Create.ForeignKey("FK_role_users_user_id")
            .FromTable("role_users").ForeignColumn("user_id")
            .ToTable("users").PrimaryColumn("user_id");
    }
}
