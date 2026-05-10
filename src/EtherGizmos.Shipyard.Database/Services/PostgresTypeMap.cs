using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Postgres;
using System.Data;

namespace EtherGizmos.Shipyard.Services;

internal class PostgresTypeMap : IPostgresTypeMap
{
    private readonly ITypeMap _inner;

    public PostgresTypeMap(
        ITypeMap inner)
    {
        _inner = inner;
    }

    public string GetTypeMap(
        DbType type,
        int? size,
        int? precision)
    {
        if (type == DbType.String || type == DbType.AnsiString)
            return "citext";

        return _inner.GetTypeMap(type, size, precision);
    }
}
