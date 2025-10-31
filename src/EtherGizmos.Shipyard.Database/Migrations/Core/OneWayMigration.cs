using FluentMigrator;

namespace EtherGizmos.Shipyard.Migrations.Core;

/// <summary>
/// Allows migration forward, but not backward.
/// </summary>
public abstract class OneWayMigration : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        throw new InvalidOperationException("This migration cannot be reversed. To downgrade, a SQL restore must be performed.");
    }
}
