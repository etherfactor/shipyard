using EtherGizmos.Shipyard.Migrations.Core;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 12, hour: 19, minute: 00, description: "Create user principals", trackingId: 74)]
public class Migration004_CreateUserPrincipals : MigrationExtension
{
    public override void Up()
    {
        Create.Column("principal_id")
            .OnTable("users")
            .AsGuid()
            .Nullable()
            .WithDefault(SystemMethods.NewGuid);

        Execute.Sql("""
            insert into acl.principals ( principal_id, principal_type_id )
            select principal_id, 100
              from users;
            """);

        Alter.Column("principal_id")
            .OnTable("users")
            .AsGuid()
            .NotNullable();

        Create.ForeignKey("FK_users_principal_id")
            .FromTable("users").ForeignColumn("principal_id")
            .ToTable("principals").InSchema("acl").PrimaryColumn("principal_id");
    }

    public override void Down()
    {
        Execute.Sql("""
            delete from acl.principals
              where principal_id in (
                select principal_id
                  from users u
              );
            """);

        Delete.Column("principal_id")
            .FromTable("users");
    }
}
