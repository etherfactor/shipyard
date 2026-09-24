using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Extensions;
using FluentMigrator;
using System.Data;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000165;

[CreatedAt(year: 2026, month: 09, day: 16, hour: 18, minute: 00, description: "Create order tables", trackingId: 165)]
public class Migration001_CreateOrderTables : AutoReversingMigration
{
    public override void Up()
    {
        /*
         * Create [dbo].[orders]
         */
        Create.Table("orders")
            .WithColumn("order_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("source").AsString(200).Nullable()
            .WithColumn("order_number").AsString(50).Nullable()
            .WithColumn("ordered_on").AsDate().Nullable()
            .WithColumn("total_price").AsDecimal(18, 3).Nullable()
            .WithColumn("order_url").AsString(int.MaxValue).Nullable()
            .WithColumn("expected_ship_start_on").AsDate().Nullable()
            .WithColumn("expected_ship_end_on").AsDate().Nullable();

        Create.AuditTriggerV1("orders", ("order_id", DbType.Int32));

        /*
         * Create [dbo].[order_items]
         */
        Create.Table("order_items")
            .WithColumn("order_item_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("order_id").AsInt32().NotNullable()
            .WithColumn("sequence").AsInt32().NotNullable()
            .WithColumn("description").AsString(int.MaxValue).NotNullable()
            .WithColumn("quantity").AsDecimal(12, 3).NotNullable()
            .WithColumn("quantity_units").AsString(50).Nullable()
            .WithColumn("unit_price").AsDecimal(18, 3).Nullable()
            .WithColumn("total_price").AsDecimal(18, 3).Nullable();

        Create.AuditTriggerV1("order_items", ("order_item_id", DbType.Int32));

        Create.ForeignKey("FK_order_items_order_id")
            .FromTable("order_items").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("order_id");

        Create.Index("IX_order_items_order_id_sequence")
            .OnTable("order_items")
            .OnColumn("order_id").Ascending()
            .OnColumn("sequence").Ascending();

        /*
         * Create [dbo].[order_fulfillment_status_types]
         */
        Create.Table("order_fulfillment_status_types")
            .WithColumn("order_fulfillment_status_type_id").AsInt32().PrimaryKey()
            .WithAuditColumns()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();

        Create.AuditTriggerV1("order_fulfillment_status_types", ("order_fulfillment_status_type_id", DbType.Int32));

        /*
         * Create [dbo].[order_fulfillments]
         */
        Create.Table("order_fulfillments")
            .WithColumn("order_fulfillment_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("order_id").AsInt32().NotNullable()
            .WithColumn("sender").AsString(200).Nullable()
            .WithColumn("order_fulfillment_status_type_id").AsInt32().NotNullable()
            .WithColumn("shipped_on").AsDate().Nullable()
            .WithColumn("received_at_utc").AsDateTime2().Nullable()
            .WithColumn("received_by_user_id").AsGuid().Nullable()
            .WithColumn("resolved_at_utc").AsDateTime2().Nullable()
            .WithColumn("resolved_by_user_id").AsGuid().Nullable()
            .WithColumn("resolved_reason").AsString(int.MaxValue).Nullable();

        Create.AuditTriggerV1("order_fulfillments", ("order_fulfillment_id", DbType.Int32));

        Create.ForeignKey("FK_order_fulfillments_order_id")
            .FromTable("order_fulfillments").ForeignColumn("order_id")
            .ToTable("orders").PrimaryColumn("order_id");

        Create.Index("IX_order_fulfillments_order_id")
            .OnTable("order_fulfillments")
            .OnColumn("order_id").Ascending();

        Create.Index("IX_order_fulfillments_sender")
            .OnTable("order_fulfillments")
            .OnColumn("sender").Ascending();

        Create.ForeignKey("FK_order_fulfillments_order_fulfillment_status_type_id")
            .FromTable("order_fulfillments").ForeignColumn("order_fulfillment_status_type_id")
            .ToTable("order_fulfillment_status_types").PrimaryColumn("order_fulfillment_status_type_id");

        Create.Index("IX_order_fulfillments_order_fulfillment_status_type_id")
            .OnTable("order_fulfillments")
            .OnColumn("order_fulfillment_status_type_id").Ascending();

        Create.ForeignKey("FK_order_fulfillments_received_by_user_id")
            .FromTable("order_fulfillments").ForeignColumn("received_by_user_id")
            .ToTable("users").PrimaryColumn("user_id");

        Create.Index("IX_order_fulfillments_received_at_utc")
            .OnTable("order_fulfillments")
            .OnColumn("received_at_utc").Ascending();

        Create.ForeignKey("FK_order_fulfillments_resolved_by_user_id")
            .FromTable("order_fulfillments").ForeignColumn("resolved_by_user_id")
            .ToTable("users").PrimaryColumn("user_id");

        Create.Index("IX_order_fulfillments_resolved_at_utc")
            .OnTable("order_fulfillments")
            .OnColumn("resolved_at_utc").Ascending();

        /*
         * Create [dbo].[order_fulfillment_items]
         */
        Create.Table("order_fulfillment_items")
            .WithColumn("order_fulfillment_item_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("order_fulfillment_id").AsInt32().NotNullable()
            .WithColumn("order_item_id").AsInt32().NotNullable()
            .WithColumn("expected_quantity").AsDecimal(12, 3).NotNullable()
            .WithColumn("received_quantity").AsDecimal(12, 3).Nullable();

        Create.AuditTriggerV1("order_fulfillment_items", ("order_fulfillment_item_id", DbType.Int32));

        Create.ForeignKey("FK_order_fulfillment_items_order_fulfillment_id")
            .FromTable("order_fulfillment_items").ForeignColumn("order_fulfillment_id")
            .ToTable("order_fulfillments").PrimaryColumn("order_fulfillment_id");

        Create.Index("IX_order_fulfillment_items_order_fulfillment_id")
            .OnTable("order_fulfillment_items")
            .OnColumn("order_fulfillment_id").Ascending();

        Create.ForeignKey("FK_order_fulfillment_items_order_item_id")
            .FromTable("order_fulfillment_items").ForeignColumn("order_item_id")
            .ToTable("order_items").PrimaryColumn("order_item_id");

        Create.Index("IX_order_fulfillment_items_order_item_id")
            .OnTable("order_fulfillment_items")
            .OnColumn("order_item_id").Ascending();

        /*
         * Create [dbo].[package_items]
         */
        Create.Table("package_items")
            .WithColumn("package_item_id").AsInt32().PrimaryKey().Identity()
            .WithAuditColumns()
            .WithColumn("package_id").AsInt32().NotNullable()
            .WithColumn("sequence").AsInt32().NotNullable()
            .WithColumn("order_fulfillment_item_id").AsInt32().Nullable()
            .WithColumn("description").AsString(int.MaxValue).NotNullable()
            .WithColumn("quantity").AsDecimal(12, 3).NotNullable()
            .WithColumn("quantity_units").AsString(50).Nullable();

        Create.AuditTriggerV1("package_items", ("package_item_id", DbType.Int32));

        Create.ForeignKey("FK_package_items_package_id")
            .FromTable("package_items").ForeignColumn("package_id")
            .ToTable("packages").PrimaryColumn("package_id");

        Create.Index("IX_package_items_package_id_sequence")
            .OnTable("package_items")
            .OnColumn("package_id").Ascending()
            .OnColumn("sequence").Ascending();

        Create.ForeignKey("FK_package_items_order_fulfillment_item_id")
            .FromTable("package_items").ForeignColumn("order_fulfillment_item_id")
            .ToTable("order_fulfillment_items").PrimaryColumn("order_fulfillment_item_id");

        Create.Index("IX_package_items_order_fulfillment_item_id")
            .OnTable("package_items")
            .OnColumn("order_fulfillment_item_id").Ascending();

        /*
         * Alter [dbo].[packages]
         */
        Alter.Table("packages")
            .AddColumn("is_itemized").AsBoolean().NotNullable().WithDefaultValue(false)
            .AddColumn("order_fulfillment_id").AsInt32().Nullable()
            .AddColumn("received_at_utc").AsDateTime2().Nullable()
            .AddColumn("received_by_user_id").AsGuid().Nullable()
            .AddColumn("resolved_at_utc").AsDateTime2().Nullable()
            .AddColumn("resolved_by_user_id").AsGuid().Nullable()
            .AddColumn("resolved_reason").AsString(int.MaxValue).Nullable();

        Create.ForeignKey("FK_packages_order_fulfillment_id")
            .FromTable("packages").ForeignColumn("order_fulfillment_id")
            .ToTable("order_fulfillments").PrimaryColumn("order_fulfillment_id");

        Create.Index("IX_packages_order_fulfillment_id")
            .OnTable("packages")
            .OnColumn("order_fulfillment_id").Ascending();

        Create.ForeignKey("FK_packages_received_by_user_id")
            .FromTable("packages").ForeignColumn("received_by_user_id")
            .ToTable("users").PrimaryColumn("user_id");

        Create.Index("IX_packages_received_at_utc")
            .OnTable("packages")
            .OnColumn("received_at_utc").Ascending();

        Create.ForeignKey("FK_packages_resolved_by_user_id")
            .FromTable("packages").ForeignColumn("resolved_by_user_id")
            .ToTable("users").PrimaryColumn("user_id");

        Create.Index("IX_packages_resolved_at_utc")
            .OnTable("packages")
            .OnColumn("resolved_at_utc").Ascending();
    }
}
