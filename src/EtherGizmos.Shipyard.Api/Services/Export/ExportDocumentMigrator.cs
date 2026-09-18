using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Exceptions;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Services.Export;

internal class ExportDocumentMigrator : IExportDocumentMigrator
{
    private readonly IReadOnlyDictionary<string, IExportDocumentMigration> _migrations;
    private readonly IReadOnlyDictionary<string, int> _maxVersion;

    public ExportDocumentMigrator(
        IEnumerable<IExportDocumentMigration> migrations)
    {
        _migrations = migrations.ToDictionary(
            e => $"{e.Kind}:{e.FromVersion}",
            e => e,
            StringComparer.OrdinalIgnoreCase);

        var versions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var migration in migrations)
        {
            versions.TryGetValue(migration.Kind, out var current);
            if (current < migration.ToVersion)
            {
                versions[migration.Kind] = migration.ToVersion;
            }
        }

        _maxVersion = versions.AsReadOnly();
    }

    public ExportDocument MigrateDataToCurrent(
        ExportDocument document)
    {
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new ObjectToInferredTypesConverter());

        var version = document.SchemaVersion;
        var maxVersion = _maxVersion.TryGetValue(document.Kind, out var mv) ? mv : 1; //Why would we ever start with V2?

        if (version > maxVersion)
        {
            throw new UnsupportedExportSchemaException(
                document.Kind,
                document.SchemaVersion,
                maxVersion,
                $"This application only supports up to schema version {maxVersion}.");
        }

        var node = JsonSerializer.SerializeToNode(document.Data, jsonOptions)!.AsObject();

        while (version < maxVersion)
        {
            if (!_migrations.TryGetValue($"{document.Kind}:{version}", out var migration))
            {
                throw new UnsupportedExportSchemaException(
                    document.Kind,
                    document.SchemaVersion,
                    maxVersion,
                    $"No schema migrators exist that can migrate schema version {document.SchemaVersion} to {maxVersion}.");
            }

            node = migration.Migrate(node);
            version = migration.ToVersion;
        }

        return document with
        {
            SchemaVersion = version,
            Data = JsonSerializer.Deserialize<IDictionary<string, object?>>(node)!,
        };
    }
}
