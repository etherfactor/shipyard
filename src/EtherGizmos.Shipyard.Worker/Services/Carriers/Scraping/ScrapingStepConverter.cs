using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public class ScrapingStepConverter : JsonConverter<ScrapingStep>
{
    public override ScrapingStep? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("stepType", out var typeProp))
            throw new JsonException("Missing 'stepType' property.");

        var type = typeProp.GetString();
        return type?.ToLowerInvariant() switch
        {
            "click" => JsonSerializer.Deserialize<ClickStep>(root.GetRawText(), options),
            "extract" => JsonSerializer.Deserialize<ExtractStep>(root.GetRawText(), options),
            "extractlist" => JsonSerializer.Deserialize<ExtractListStep>(root.GetRawText(), options),
            "navigate" => JsonSerializer.Deserialize<NavigateStep>(root.GetRawText(), options),
            "replace" => JsonSerializer.Deserialize<ReplaceStep>(root.GetRawText(), options),
            "return" => JsonSerializer.Deserialize<ReturnStep>(root.GetRawText(), options),
            "script" => JsonSerializer.Deserialize<ScriptStep>(root.GetRawText(), options),
            "send" => JsonSerializer.Deserialize<SendStep>(root.GetRawText(), options),
            "set" => JsonSerializer.Deserialize<SetStep>(root.GetRawText(), options),
            "waitfor" => JsonSerializer.Deserialize<WaitForStep>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown step type '{type}'.")
        };

    }

    public override void Write(Utf8JsonWriter writer, ScrapingStep value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
