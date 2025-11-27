using EtherGizmos.Shipyard.Migrations.Core;

namespace EtherGizmos.Shipyard.Migrations._00._00.Feature000074;

[CreatedAt(year: 2025, month: 11, day: 20, hour: 18, minute: 00, description: "Create securable access views", trackingId: 74)]
public class Migration010_CreateSecurableAccessViews : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [acl].[carriers]
         */
        Execute.Sql("""
            create view acl.carriers as
            with global_permissions as (
              select a.principal_user_id,
                r.carrier_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from carriers r
                  inner join acl.user_entries a
                    on a.securable_type_id = 10
            ),
            record_permissions as (
              select a.principal_user_id,
                r.carrier_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from carriers r
                  inner join acl.user_entries a
                    on a.securable_id = r.securable_id
            ),
            all_permissions_base as (
              select gp.principal_user_id,
                gp.carrier_id,
                gp.permission_id,
                gp.permission_grant_type_id,
                gp.priority
                from global_permissions gp
              union all
              select rp.principal_user_id,
                rp.carrier_id,
                rp.permission_id,
                rp.permission_grant_type_id,
                rp.priority
                from record_permissions rp
            ),
            all_permissions as (
              select ap.principal_user_id,
                ap.carrier_id,
                ap.permission_id,
                ap.permission_grant_type_id,
                ap.priority,
                row_number() over (
                  partition by ap.principal_user_id,
                    ap.carrier_id,
            	    ap.permission_id
                  order by ap.priority desc
                ) as rownum
                from all_permissions_base ap
            )
            select p.principal_user_id,
              p.carrier_id,
              p.permission_id,
              p.permission_grant_type_id,
              case when p.permission_grant_type_id = 1 then 1
                when p.permission_grant_type_id = 2 then 1
               	else 0 end as is_grant
              from all_permissions p
                inner join carriers r
               	  on r.carrier_id = p.carrier_id
              where p.rownum = 1;
            """);

        /*
         * Create [acl].[packages]
         */
        Execute.Sql("""
            create view acl.packages as
            with global_permissions as (
              select a.principal_user_id,
                r.package_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from packages r
                  inner join acl.user_entries a
                    on a.securable_type_id = 20
            ),
            record_permissions as (
              select a.principal_user_id,
                r.package_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from packages r
                  inner join acl.user_entries a
                    on a.securable_id = r.securable_id
            ),
            all_permissions_base as (
              select gp.principal_user_id,
                gp.package_id,
                gp.permission_id,
                gp.permission_grant_type_id,
                gp.priority
                from global_permissions gp
              union all
              select rp.principal_user_id,
                rp.package_id,
                rp.permission_id,
                rp.permission_grant_type_id,
                rp.priority
                from record_permissions rp
            ),
            all_permissions as (
              select ap.principal_user_id,
                ap.package_id,
                ap.permission_id,
                ap.permission_grant_type_id,
                ap.priority,
                row_number() over (
                  partition by ap.principal_user_id,
                    ap.package_id,
            	    ap.permission_id
                  order by ap.priority desc
                ) as rownum
                from all_permissions_base ap
            )
            select p.principal_user_id,
              p.package_id,
              p.permission_id,
              p.permission_grant_type_id,
              case when p.permission_grant_type_id = 1 then 1
                when p.permission_grant_type_id = 2 and (
                  r.created_by_user_id = p.principal_user_id
                  or r.group_id = u.group_id
                ) then 1
               	else 0 end as is_grant
              from all_permissions p
                inner join packages r
               	  on r.package_id = p.package_id
                inner join users u
                  on u.user_id = p.principal_user_id
              where p.rownum = 1;
            """);

        /*
         * Create [acl].[users]
         */
        Execute.Sql("""
            create view acl.users as
            with global_permissions as (
              select a.principal_user_id,
                r.user_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from users r
                  inner join acl.user_entries a
                    on a.securable_type_id = 100
            ),
            record_permissions as (
              select a.principal_user_id,
                r.user_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from users r
                  inner join acl.user_entries a
                    on a.securable_id = r.securable_id
            ),
            all_permissions_base as (
              select gp.principal_user_id,
                gp.user_id,
                gp.permission_id,
                gp.permission_grant_type_id,
                gp.priority
                from global_permissions gp
              union all
              select rp.principal_user_id,
                rp.user_id,
                rp.permission_id,
                rp.permission_grant_type_id,
                rp.priority
                from record_permissions rp
            ),
            all_permissions as (
              select ap.principal_user_id,
                ap.user_id,
                ap.permission_id,
                ap.permission_grant_type_id,
                ap.priority,
                row_number() over (
                  partition by ap.principal_user_id,
                    ap.user_id,
            	    ap.permission_id
                  order by ap.priority desc
                ) as rownum
                from all_permissions_base ap
            )
            select p.principal_user_id,
              p.user_id,
              p.permission_id,
              p.permission_grant_type_id,
              case when p.permission_grant_type_id = 1 then 1
                when p.permission_grant_type_id = 2 and (
                  r.group_id = u.group_id
                ) then 1
               	else 0 end as is_grant
              from all_permissions p
                inner join users r
               	  on r.user_id = p.user_id
                inner join users u
                  on u.user_id = p.principal_user_id
              where p.rownum = 1;
            """);

        /*
         * Create [acl].[roles]
         */
        Execute.Sql("""
            create view acl.roles as
            with global_permissions as (
              select a.principal_user_id,
                r.role_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from roles r
                  inner join acl.user_entries a
                    on a.securable_type_id = 110
            ),
            record_permissions as (
              select a.principal_user_id,
                r.role_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from roles r
                  inner join acl.user_entries a
                    on a.securable_id = r.securable_id
            ),
            all_permissions_base as (
              select gp.principal_user_id,
                gp.role_id,
                gp.permission_id,
                gp.permission_grant_type_id,
                gp.priority
                from global_permissions gp
              union all
              select rp.principal_user_id,
                rp.role_id,
                rp.permission_id,
                rp.permission_grant_type_id,
                rp.priority
                from record_permissions rp
            ),
            all_permissions as (
              select ap.principal_user_id,
                ap.role_id,
                ap.permission_id,
                ap.permission_grant_type_id,
                ap.priority,
                row_number() over (
                  partition by ap.principal_user_id,
                    ap.role_id,
            	    ap.permission_id
                  order by ap.priority desc
                ) as rownum
                from all_permissions_base ap
            )
            select p.principal_user_id,
              p.role_id,
              p.permission_id,
              p.permission_grant_type_id,
              case when p.permission_grant_type_id = 1 then 1
                when p.permission_grant_type_id = 2 then 1
               	else 0 end as is_grant
              from all_permissions p
                inner join roles r
               	  on r.role_id = p.role_id
              where p.rownum = 1;
            """);

        /*
         * Create [acl].[groups]
         */
        Execute.Sql("""
            create view acl.groups as
            with global_permissions as (
              select a.principal_user_id,
                r.group_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from groups r
                  inner join acl.user_entries a
                    on a.securable_type_id = 120
            ),
            record_permissions as (
              select a.principal_user_id,
                r.group_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from groups r
                  inner join acl.user_entries a
                    on a.securable_id = r.securable_id
            ),
            all_permissions_base as (
              select gp.principal_user_id,
                gp.group_id,
                gp.permission_id,
                gp.permission_grant_type_id,
                gp.priority
                from global_permissions gp
              union all
              select rp.principal_user_id,
                rp.group_id,
                rp.permission_id,
                rp.permission_grant_type_id,
                rp.priority
                from record_permissions rp
            ),
            all_permissions as (
              select ap.principal_user_id,
                ap.group_id,
                ap.permission_id,
                ap.permission_grant_type_id,
                ap.priority,
                row_number() over (
                  partition by ap.principal_user_id,
                    ap.group_id,
            	    ap.permission_id
                  order by ap.priority desc
                ) as rownum
                from all_permissions_base ap
            )
            select p.principal_user_id,
              p.group_id,
              p.permission_id,
              p.permission_grant_type_id,
              case when p.permission_grant_type_id = 1 then 1
                when p.permission_grant_type_id = 2 and (
                  r.group_id = u.group_id
                ) then 1
               	else 0 end as is_grant
              from all_permissions p
                inner join groups r
               	  on r.group_id = p.group_id
                inner join users u
                  on u.user_id = p.principal_user_id
              where p.rownum = 1;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            drop view acl.groups;
            """);

        Execute.Sql("""
            drop view acl.roles;
            """);

        Execute.Sql("""
            drop view acl.users;
            """);

        Execute.Sql("""
            drop view acl.packages;
            """);

        Execute.Sql("""
            drop view acl.carriers;
            """);
    }
}
