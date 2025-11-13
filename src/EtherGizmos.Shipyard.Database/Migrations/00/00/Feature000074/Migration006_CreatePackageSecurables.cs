using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 12, hour: 19, minute: 40, description: "Create package securables", trackingId: 74)]
public class Migration006_CreatePackageSecurables : MigrationExtension
{
    public override void Up()
    {
        Create.Column("securable_id")
            .OnTable("packages")
            .AsGuid()
            .Nullable()
            .WithDefault(SystemMethods.NewGuid);

        Execute.Sql("""
            insert into acl.securables ( securable_id, securable_type_id )
            select securable_id, 20
              from packages;
            """);

        Alter.Column("securable_id")
            .OnTable("packages")
            .AsGuid()
            .NotNullable();

        Create.ForeignKey("FK_packages_securable_id")
            .FromTable("packages").ForeignColumn("securable_id")
            .ToTable("securables").InSchema("acl").PrimaryColumn("securable_id");
    }

    public override void Down()
    {
        Execute.Sql("""
            delete from acl.securables
              where securable_id in (
                select securable_id
                  from packages u
              );
            """);

        Delete.Column("securable_id")
            .FromTable("packages");
    }
}
