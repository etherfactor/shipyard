using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 15, hour: 18, minute: 00, description: "Create group tables", trackingId: 74)]
public class Migration009_CreateGroupTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[groups]
         */
        Create.Table("groups")
            .WithColumn("group_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("system_id").AsGuid().Nullable()
            .WithColumn("securable_id").AsGuid().NotNullable();

        Create.ForeignKey("FK_groups_securable_id")
            .FromTable("groups").ForeignColumn("securable_id")
            .ToTable("securables").InSchema("acl").PrimaryColumn("securable_id");

        /*
         * Add [group_id] to [dbo].[users]
         */
        Create.Column("group_id")
            .OnTable("users")
            .AsInt32()
            .Nullable();

        Create.Index("IX_users_group_id")
            .OnTable("users")
            .OnColumn("group_id")
            .Ascending();

        Create.ForeignKey("FK_users_group_id")
            .FromTable("users").ForeignColumn("group_id")
            .ToTable("groups").PrimaryColumn("group_id");

        /*
         * Add [group_id] to [dbo].[packages]
         */
        Create.Column("group_id")
            .OnTable("packages")
            .AsInt32()
            .Nullable();

        Create.Index("IX_packages_group_id")
            .OnTable("packages")
            .OnColumn("group_id")
            .Ascending();

        Create.ForeignKey("FK_packages_group_id")
            .FromTable("packages").ForeignColumn("group_id")
            .ToTable("groups").PrimaryColumn("group_id");
    }
}
