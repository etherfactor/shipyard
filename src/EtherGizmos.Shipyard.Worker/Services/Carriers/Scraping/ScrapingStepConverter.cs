using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public class ScrapingStepConverter : JsonConverter<ScrapingStep>
{
    public override ScrapingStep? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
            throw new JsonException("Missing 'type' property.");

        var type = typeProp.GetString();
        return type?.ToLowerInvariant() switch
        {
            nameof(ScrapingStepType.Extract) => JsonSerializer.Deserialize<ExtractStep>(root.GetRawText(), options),
            nameof(ScrapingStepType.Navigate) => JsonSerializer.Deserialize<NavigateStep>(root.GetRawText(), options),
            nameof(ScrapingStepType.Return) => JsonSerializer.Deserialize<ReturnStep>(root.GetRawText(), options),
            nameof(ScrapingStepType.Set) => JsonSerializer.Deserialize<SetStep>(root.GetRawText(), options),
            nameof(ScrapingStepType.WaitFor) => JsonSerializer.Deserialize<WaitForStep>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown step type '{type}'.")
        };

    }

    public override void Write(Utf8JsonWriter writer, ScrapingStep value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
