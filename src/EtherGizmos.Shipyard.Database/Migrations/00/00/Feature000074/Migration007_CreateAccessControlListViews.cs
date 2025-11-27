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
            with role_permissions as (
              select a.entry_id,
                u.user_id as principal_user_id,
                a.securable_id,
                a.securable_type_id,
                a.permission_id,
                a.permission_grant_type_id,
            	--Role permissions get +0
            	0
            	--Record permissions get +10000
            	+ case when a.securable_id is not null then 10000
            	  else 0 end
            	--Priority permissions get +100000
            	+ case when a.is_priority is true then 100000
            	  else 0 end
            	--Additional bonus depending on grant type
            	+ case when a.permission_grant_type_id = -1 then 100
            	  when a.permission_grant_type_id = 1 then 10
            	  when a.permission_grant_type_id = 2 then 1
            	  else 0 end
            	  as priority
                from users u
                  inner join role_users ru
                    on ru.user_id = u.user_id
                  inner join roles r
                    on r.role_id = ru.role_id
                  inner join acl.entries a
                    on a.principal_id = r.principal_id
            ),
            user_permissions as (
              select a.entry_id,
                u.user_id as principal_user_id,
                a.securable_id,
                a.securable_type_id,
                a.permission_id,
                a.permission_grant_type_id,
            	--User permissions get +1000
            	1000
            	--Record permissions get +10000
            	+ case when a.securable_id is not null then 10000
            	  else 0 end
            	--Priority permissions get +100000
            	+ case when a.is_priority is true then 100000
            	  else 0 end
            	--Additional bonus depending on grant type
            	+ case when a.permission_grant_type_id = -1 then 100
            	  when a.permission_grant_type_id = 1 then 10
            	  when a.permission_grant_type_id = 2 then 1
            	  else 0 end
            	  as priority
                from users u
                  inner join acl.entries a
                    on a.principal_id = u.principal_id
            ),
            all_permissions_base as (
              select rp.entry_id,
                rp.principal_user_id,
                rp.securable_id,
                rp.securable_type_id,
                rp.permission_id,
                rp.permission_grant_type_id,
            	rp.priority
            	from role_permissions rp
              union all
              select up.entry_id,
                up.principal_user_id,
                up.securable_id,
                up.securable_type_id,
                up.permission_id,
                up.permission_grant_type_id,
            	up.priority
                from user_permissions up
            ),
            all_permissions as (
              select ap.entry_id,
                ap.principal_user_id,
                ap.securable_id,
                ap.securable_type_id,
                ap.permission_id,
                ap.permission_grant_type_id,
            	ap.priority,
            	row_number() over (
                  partition by ap.principal_user_id,
                    ap.securable_id,
                    ap.securable_type_id,
                    ap.permission_id
            	  order by ap.priority desc
            	) as rownum
            	from all_permissions_base ap
            )
            select ap.entry_id,
              ap.principal_user_id,
              ap.securable_id,
              ap.securable_type_id,
              ap.permission_id,
              ap.permission_grant_type_id,
              ap.priority
              from all_permissions ap
              where ap.rownum = 1;
            """);

        /*
         * Create [acl].[user_capabilities]
         */
        Execute.Sql("""
            create view acl.user_capabilities as
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
            drop view acl.user_capabilities;
            """);

        Execute.Sql("""
            drop view acl.user_entries;
            """);
    }
}
