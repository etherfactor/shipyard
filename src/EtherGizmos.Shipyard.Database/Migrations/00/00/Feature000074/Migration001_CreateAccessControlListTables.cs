using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 12, hour: 18, minute: 00, description: "Create access control list tables", trackingId: 74)]
public class Migration001_CreateAccessControlListTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [acl]
         */
        Create.Schema("acl");

        /*
         * Create [acl].[permissions]
         */
        Create.Table("permissions").InSchema("acl")
            .WithColumn("permission_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [acl].[permission_grant_types]
         */
        Create.Table("permission_grant_types").InSchema("acl")
            .WithColumn("permission_grant_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [acl].[principal_types]
         */
        Create.Table("principal_types").InSchema("acl")
            .WithColumn("principal_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [acl].[principals]
         */
        Create.Table("principals").InSchema("acl")
            .WithColumn("principal_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("principal_type_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_principals_principal_type_id")
            .FromTable("principals").InSchema("acl").ForeignColumn("principal_type_id")
            .ToTable("principal_types").InSchema("acl").PrimaryColumn("principal_type_id");

        /*
         * Create [acl].[securable_types]
         */
        Create.Table("securable_types").InSchema("acl")
            .WithColumn("securable_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        /*
         * Create [acl].[securables]
         */
        Create.Table("securables").InSchema("acl")
            .WithColumn("securable_id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("securable_type_id").AsInt32().NotNullable();

        Create.ForeignKey("FK_securables_securable_type_id")
            .FromTable("securables").InSchema("acl").ForeignColumn("securable_type_id")
            .ToTable("securable_types").InSchema("acl").PrimaryColumn("securable_type_id");

        /*
         * Create [acl].[entries]
         */
        Create.Table("entries").InSchema("acl")
            .WithColumn("entry_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("principal_id").AsGuid().NotNullable()
            .WithColumn("securable_id").AsGuid().Nullable()
            .WithColumn("securable_type_id").AsInt32().Nullable()
            .WithColumn("permission_id").AsInt32().NotNullable()
            .WithColumn("permission_grant_type_id").AsInt32().NotNullable()
            .WithColumn("is_priority").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Index("IX_entries_principal_id")
            .OnTable("entries").InSchema("acl")
            .OnColumn("principal_id")
            .Ascending();

        Create.Index("IX_entries_securable_id")
            .OnTable("entries").InSchema("acl")
            .OnColumn("securable_id")
            .Ascending();

        Create.Index("IX_entries_securable_type_id")
            .OnTable("entries").InSchema("acl")
            .OnColumn("securable_type_id")
            .Ascending();

        Create.Index("UX_entries_principal_id_securable_id_permission_id")
            .OnTable("entries").InSchema("acl")
            .OnColumn("principal_id")
            .Unique()
            .OnColumn("securable_id")
            .Unique()
            .OnColumn("permission_id")
            .Unique();

        Create.Index("UX_entries_principal_id_securable_type_id_permission_id")
            .OnTable("entries").InSchema("acl")
            .OnColumn("principal_id")
            .Unique()
            .OnColumn("securable_type_id")
            .Unique()
            .OnColumn("permission_id")
            .Unique();

        Create.ForeignKey("FK_entries_principal_id")
            .FromTable("entries").InSchema("acl").ForeignColumn("principal_id")
            .ToTable("principals").InSchema("acl").PrimaryColumn("principal_id")
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_entries_securable_id")
            .FromTable("entries").InSchema("acl").ForeignColumn("securable_id")
            .ToTable("securables").InSchema("acl").PrimaryColumn("securable_id");

        Create.ForeignKey("FK_entries_permission_id")
            .FromTable("entries").InSchema("acl").ForeignColumn("permission_id")
            .ToTable("permissions").InSchema("acl").PrimaryColumn("permission_id");

        Create.ForeignKey("FK_entries_permission_grant_type_id")
            .FromTable("entries").InSchema("acl").ForeignColumn("permission_grant_type_id")
            .ToTable("permission_grant_types").InSchema("acl").PrimaryColumn("permission_grant_type_id");
    }
}
