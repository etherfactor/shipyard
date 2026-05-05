using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard;

internal static class JsonOptions
{
    public static JsonSerializerOptions Default { get; }

    static JsonOptions()
    {
        Default = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        Default.Converters.Add(new JsonStringEnumConverter());
    }
}
