using EtherGizmos.Common.Messaging.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Common.Messaging.Services;

public class JsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _options;

    public JsonMessageSerializer()
    {
        _options = new();
        _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        _options.Converters.Add(new JsonStringEnumConverter());
    }

    public TMessage Deserialize<TMessage>(
        string message)
        where TMessage : class, new()
    {
        return JsonSerializer.Deserialize<TMessage>(message, _options)!;
    }

    public string Serialize<TMessage>(
        TMessage message)
        where TMessage : class, new()
    {
        return JsonSerializer.Serialize(message, _options);
    }
}
