using System.Text.Json.Nodes;
using VYaml.Annotations;

namespace EtherGizmos.Shipyard.Api.Models;

[YamlObject]
public partial class ExportDocument
{
    [YamlConstructor]
    public ExportDocument() { }

    public ExportDocument(
        string kind,
        int schemaVersion,
        IDictionary<string, object?>? metadata,
        IDictionary<string, object?> data)
    {
        Kind = kind;
        SchemaVersion = schemaVersion;
        Metadata = metadata;
        Data = data;
    }

    public string Kind { get; init; } = null!;

    public int SchemaVersion { get; init; }

    public IDictionary<string, object?>? Metadata { get; init; }

    public IDictionary<string, object?> Data { get; init; } = null!;
}
