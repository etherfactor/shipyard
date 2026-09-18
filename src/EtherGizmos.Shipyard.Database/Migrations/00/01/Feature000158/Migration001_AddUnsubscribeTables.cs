using EtherGizmos.Common.Abstractions;
using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations._00._01.Feature000158;

[CreatedAt(year: 2026, month: 08, day: 19, hour: 18, minute: 00, description: "Create unsubscribe tables", trackingId: 158)]
public class Migration001_AddUnsubscribeTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("unsubscribe_keys").InSchema("notification")
            .WithColumn("unsubscribe_key_id").AsInt64().PrimaryKey().Identity()
            .WithColumn("subscription_id").AsInt64().NotNullable()
            .WithColumn("value").AsBinary(32).NotNullable();

        Create.ForeignKey("FK_unsubscribe_keys_subscription_id")
            .FromTable("unsubscribe_keys").InSchema("notification").ForeignColumn("subscription_id")
            .ToTable("subscriptions").InSchema("notification").PrimaryColumn("subscription_id");

        Create.Index("UX_unsubscribe_keys_subscription_id")
            .OnTable("unsubscribe_keys").InSchema("notification")
            .OnColumn("subscription_id").Unique();
    }
}
