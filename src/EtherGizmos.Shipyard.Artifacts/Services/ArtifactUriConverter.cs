using EtherGizmos.Common.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Services;

public class ArtifactUriConverter : JsonConverter<ArtifactUri>
{
    public override ArtifactUri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new ArtifactUri(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, ArtifactUri value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
