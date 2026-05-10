using EtherGizmos.Common.Converters;
using System.Text.Json.Serialization;

#pragma warning disable IDE0130
namespace System.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    private static readonly JsonSerializerOptions _export;

    static JsonSerializerOptionsExtensions()
    {
        _export = new(JsonSerializerOptions.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(),
                new ObjectToInferredTypesConverter(),
            },
        };
    }

    extension(JsonSerializerOptions)
    {
        public static JsonSerializerOptions Export => _export;
    }
}
