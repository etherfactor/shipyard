using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Abstractions;

public interface IExportDocumentMigration
{
    string Kind { get; }

    int FromVersion { get; }

    int ToVersion { get; }

    JsonObject Migrate(JsonObject old);
}
