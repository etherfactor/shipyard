using EtherGizmos.Common.Utilities.Converters;
using EtherGizmos.Shipyard.Models.Database.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Models.Database;

public class CarrierRunbookStep
{
    private static readonly JsonSerializerOptions _jsonOptions;

    static CarrierRunbookStep()
    {
        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        _jsonOptions.Converters.Add(new ObjectToInferredTypesConverter());
    }

    public virtual int Id { get; set; }

    public virtual int CarrierId { get; set; }

    [JsonIgnore]
    public virtual Carrier Carrier { get; set; } = null!;

    public virtual StepType StepType { get; set; }

    public virtual IDictionary<string, object> Payload { get; set; } = new Dictionary<string, object>();
}
