using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 13, hour: 18, minute: 00, description: "Create access control list views", trackingId: 74)]
public class Migration007_CreateAccessControlListViews : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [acl].[users]
         */
        Execute.Sql("""
            create view acl.user_entries as
            with role_permissions as (
              select u.user_id as principal_user_id,
                a.securable_id,
                a.securable_type_id,
                a.permission_id,
                a.permission_grant_type_id
                from users u
                  inner join role_users ru
                    on ru.user_id = u.user_id
                  inner join roles r
                    on r.role_id = ru.role_id
                  inner join acl.entries a
                    on a.principal_id = r.principal_id
            ),
            user_permissions as (
              select u.user_id as principal_user_id,
                a.securable_id,
                a.securable_type_id,
                a.permission_id,
                a.permission_grant_type_id
                from users u
                  inner join acl.entries a
                    on a.principal_id = u.principal_id
            )
            select rp.principal_user_id,
              rp.securable_id,
              rp.securable_type_id,
              rp.permission_id,
              rp.permission_grant_type_id
              from role_permissions rp
                where not exists (
                  select 1
                    from user_permissions up
                      where up.principal_user_id = rp.principal_user_id
                        and up.permission_id = rp.permission_id
                        and (
                          ( up.securable_id = rp.securable_id )
                          or ( up.securable_type_id = rp.securable_type_id )
                        )
                )
            union all
            select up.principal_user_id,
              up.securable_id,
              up.securable_type_id,
              up.permission_id,
              up.permission_grant_type_id
              from user_permissions up;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            drop view acl.users;
            """);
    }
}
