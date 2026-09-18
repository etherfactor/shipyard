using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Services.Carriers.Scraping;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Extensions;

internal static class JsonSerializerOptionsExtensions
{
    private static readonly JsonSerializerOptions _app;

    static JsonSerializerOptionsExtensions()
    {
        _app = new(JsonSerializerOptions.Web);
        _app.Converters.Add(new JsonStringEnumConverter());
        _app.Converters.Add(new ObjectToInferredTypesConverter());
        _app.Converters.Add(new ScrapingStepConverter());
    }

    extension(JsonSerializerOptions)
    {
        public static JsonSerializerOptions App => _app;
    }
}
