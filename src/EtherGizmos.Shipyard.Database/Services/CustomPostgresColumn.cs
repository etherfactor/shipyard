using FluentMigrator.Generation;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;
using FluentMigrator.Runner.Generators.Postgres;
using System.Reflection;

namespace EtherGizmos.Shipyard.Services;

internal class CustomPostgresColumn : ColumnBase<IPostgresTypeMap>
{
    private readonly ColumnBase<IPostgresTypeMap> _inner;

    [Obsolete]
    public CustomPostgresColumn(
        ColumnBase<IPostgresTypeMap> inner,
        IPostgresTypeMap typeMap,
        IQuoter quoter)
        : base(typeMap, quoter)
    {
        _inner = inner;
    }

    protected override string FormatIdentity(ColumnDefinition column)
        => (string)typeof(ColumnBase<IPostgresTypeMap>)
            .GetMethod(nameof(FormatIdentity), BindingFlags.NonPublic | BindingFlags.Instance, [typeof(ColumnDefinition)])!
            .Invoke(_inner, [column])!;

    protected override string FormatType(ColumnDefinition column)
    {
        //Don't use custom types for [migration_history]
        if (column.TableName == "migration_history")
            return (string)typeof(ColumnBase<IPostgresTypeMap>)
                .GetMethod(nameof(FormatType), BindingFlags.NonPublic | BindingFlags.Instance, [typeof(ColumnDefinition)])!
                .Invoke(_inner, [column])!;

        return base.FormatType(column);
    }

    public virtual string GenerateAlterClauses(ColumnDefinition column)
        => (string)_inner.GetType()
            .GetMethod(nameof(GenerateAlterClauses), BindingFlags.Public | BindingFlags.Instance, [typeof(ColumnDefinition)])!
            .Invoke(_inner, [column])!;

    public virtual string FormatAlterDefaultValue(string column, object defaultValue)
        => (string)_inner.GetType()
            .GetMethod(nameof(FormatAlterDefaultValue), BindingFlags.Public | BindingFlags.Instance, [typeof(string), typeof(object)])!
            .Invoke(_inner, [column, defaultValue])!;
}
