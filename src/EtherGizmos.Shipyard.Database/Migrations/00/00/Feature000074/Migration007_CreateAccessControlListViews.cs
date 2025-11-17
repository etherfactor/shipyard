using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 13, hour: 18, minute: 00, description: "Create access control list views", trackingId: 74)]
public class Migration007_CreateAccessControlListViews : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [acl].[user_entries]
         */
        Execute.Sql("""
            create view acl.user_entries as
            with role_permissions_base as (
              select u.user_id as principal_user_id,
                a.securable_id,
                a.securable_type_id,
                a.permission_id,
                a.permission_grant_type_id,
                row_number() over (
                  partition by u.user_id, a.securable_id, a.securable_type_id, a.permission_id
                  order by case when a.permission_grant_type_id = -1 then 1
                    when a.permission_grant_type_id = 1 then 2
                    when a.permission_grant_type_id = 2 then 3
                    else 999999 end
                ) as rownum
                from users u
                  inner join role_users ru
                    on ru.user_id = u.user_id
                  inner join roles r
                    on r.role_id = ru.role_id
                  inner join acl.entries a
                    on a.principal_id = r.principal_id
            ),
            role_permissions as (
              select rp.principal_user_id,
                rp.securable_id,
                rp.securable_type_id,
                rp.permission_id,
                rp.permission_grant_type_id
                from role_permissions_base rp
                where rp.rownum = 1
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

        /*
         * Create [acl].[user_capabilities]
         */
        Execute.Sql("""
            create view user_capabilities as
            with user_grants as (
              select distinct ue.principal_user_id,
                ue.securable_type_id,
                ue.permission_id
                from acl.user_entries ue
                where ue.securable_type_id is not null
                  and coalesce( ue.permission_grant_type_id, -1 ) <> -1
              union
              select distinct ue.principal_user_id,
                s.securable_type_id,
                ue.permission_id
                from acl.user_entries ue
                  inner join acl.securables s
                    on s.securable_id = ue.securable_id
                where ue.securable_id is not null
                  and coalesce( ue.permission_grant_type_id, -1 ) <> -1
            )
            select u.user_id as principal_user_id,
              st.securable_type_id,
              p.permission_id,
              case when ug.principal_user_id is not null then 1
                else 0 end as is_allowed
              from users u
                cross join acl.securable_types st
                cross join acl.permissions p
                left outer join user_grants ug
                  on ug.principal_user_id = u.user_id
                    and ug.securable_type_id = st.securable_type_id
                    and ug.permission_id = p.permission_id;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            drop view acl.users;
            """);
    }
}
