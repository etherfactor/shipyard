using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000165;

[CreatedAt(year: 2026, month: 10, day: 29, hour: 14, minute: 30, description: "Create securable access views for orders", trackingId: 165)]
public class Migration005_CreateSecurableAccessViews : MigrationExtension
{
    public override void Up()
    {
        /*
         * Create [acl].[orders]
         */
        Execute.Sql("""
            create view acl.orders as
            with global_permissions as (
              select a.principal_user_id,
                r.order_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from orders r
                  inner join acl.user_entries a
                    on a.securable_type_id = 30
            ),
            record_permissions as (
              select a.principal_user_id,
                r.order_id,
                a.permission_id,
                a.permission_grant_type_id,
                a.priority
                from orders r
                  inner join acl.user_entries a
                    on a.securable_id = r.securable_id
            ),
            all_permissions_base as (
              select gp.principal_user_id,
                gp.order_id,
                gp.permission_id,
                gp.permission_grant_type_id,
                gp.priority
                from global_permissions gp
              union all
              select rp.principal_user_id,
                rp.order_id,
                rp.permission_id,
                rp.permission_grant_type_id,
                rp.priority
                from record_permissions rp
            ),
            all_permissions as (
              select ap.principal_user_id,
                ap.order_id,
                ap.permission_id,
                ap.permission_grant_type_id,
                ap.priority,
                row_number() over (
                  partition by ap.principal_user_id,
                    ap.order_id,
            	    ap.permission_id
                  order by ap.priority desc
                ) as rownum
                from all_permissions_base ap
            )
            select p.principal_user_id,
              p.order_id,
              p.permission_id,
              p.permission_grant_type_id,
              case when p.permission_grant_type_id = 1 then 1
                when p.permission_grant_type_id = 2 then 1
               	else 0 end as is_grant
              from all_permissions p
                inner join orders r
               	  on r.order_id = p.order_id
              where p.rownum = 1;
            """);

        /*
         * Create [acl].[order_items]
         */
        Execute.Sql("""
            create view acl.order_items as
            select a.principal_user_id,
              r.order_item_id,
              a.permission_id,
              a.permission_grant_type_id
              from order_items r
                inner join acl.orders a
                  on a.order_id = r.order_id;
            """);

        /*
         * Create [acl].[order_fulfillments]
         */
        Execute.Sql("""
            create view acl.order_fulfillments as
            select a.principal_user_id,
              r.order_fulfillment_id,
              a.permission_id,
              a.permission_grant_type_id
              from order_fulfillments r
                inner join acl.orders a
                  on a.order_id = r.order_id;
            """);

        /*
         * Create [acl].[order_fulfillment_items]
         */
        Execute.Sql("""
            create view acl.order_fulfillment_items as
            select a.principal_user_id,
              r.order_fulfillment_item_id,
              a.permission_id,
              a.permission_grant_type_id
              from order_fulfillment_items r
                inner join acl.order_fulfillments a
                  on a.order_fulfillment_id = r.order_fulfillment_id;
            """);

        /*
         * Create [acl].[package_items]
         */
        Execute.Sql("""
            create view acl.package_items as
            select a.principal_user_id,
              r.package_item_id,
              a.permission_id,
              a.permission_grant_type_id
              from package_items r
                inner join acl.packages a
                  on a.package_id = r.package_id;
            """);
    }

    public override void Down()
    {
        Execute.Sql("""
            drop view acl.package_items;
            """);

        Execute.Sql("""
            drop view acl.order_fulfillment_items;
            """);

        Execute.Sql("""
            drop view acl.order_fulfillments;
            """);

        Execute.Sql("""
            drop view acl.order_items;
            """);

        Execute.Sql("""
            drop view acl.orders;
            """);
    }
}
