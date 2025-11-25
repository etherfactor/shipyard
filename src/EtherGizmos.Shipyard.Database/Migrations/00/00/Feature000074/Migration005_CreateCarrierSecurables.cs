using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 12, hour: 19, minute: 20, description: "Create carrier securables", trackingId: 74)]
public class Migration005_CreateCarrierSecurables : MigrationExtension
{
    public override void Up()
    {
        Create.Column("securable_id")
            .OnTable("carriers")
            .AsGuid()
            .Nullable()
            .WithDefault(SystemMethods.NewGuid);

        Execute.Sql("""
            insert into acl.securables ( securable_id, securable_type_id )
            select securable_id, 10
              from carriers;
            """);

        Alter.Column("securable_id")
            .OnTable("carriers")
            .AsGuid()
            .NotNullable();

        Create.ForeignKey("FK_packages_securable_id")
            .FromTable("carriers").ForeignColumn("securable_id")
            .ToTable("securables").InSchema("acl").PrimaryColumn("securable_id");
    }

    public override void Down()
    {
        Execute.Sql("""
            delete from acl.securables
              where securable_id in (
                select securable_id
                  from carriers u
              );
            """);

        Delete.Column("securable_id")
            .FromTable("carriers");
    }
}
