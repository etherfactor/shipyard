using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace EtherGizmos.Shipyard.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ICreateTableWithColumnSyntax"/>.
/// </summary>
internal static class ICreateTableWithColumnSyntaxExtensions
{
    /// <summary>
    /// Adds audit columns to the table.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <returns>Itself.</returns>
    internal static ICreateTableWithColumnSyntax WithAuditColumns(this ICreateTableWithColumnSyntax @this)
    {
        return @this
            .WithColumn("created_at_utc").AsDateTime2().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("created_by_user_id").AsGuid().Nullable()
            .WithColumn("modified_at_utc").AsDateTime2().Nullable()
            .WithColumn("modified_by_user_id").AsGuid().Nullable();
    }
}
