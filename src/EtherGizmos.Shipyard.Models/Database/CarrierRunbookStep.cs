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
    }

    [JsonIgnore]
    public virtual int Id { get; set; }

    [JsonIgnore]
    public virtual int CarrierId { get; set; }

    [JsonIgnore]
    public virtual Carrier Carrier { get; set; } = null!;

    public virtual StepType StepType { get; set; }

    public virtual string? From { get; set; }

    public virtual string? Name { get; set; }

    public virtual string? Selector { get; set; }

    public virtual List<CarrierRunbookStep>? Steps { get; set; }

    public virtual string? To { get; set; }

    public virtual bool? Trim { get; set; }

    public virtual string? Url { get; set; }

    public virtual string? Value { get; set; }

    public virtual string? Var { get; set; }

    [JsonIgnore]
    public virtual string Payload
    {
        get
        {
            return JsonSerializer.Serialize(this, _jsonOptions);
        }
        set
        {
            var inner = JsonSerializer.Deserialize<CarrierRunbookStep>(value, _jsonOptions)!;
            From = inner.From;
            Name = inner.Name;
            Selector = inner.Selector;
            Steps = inner.Steps;
            To = inner.To;
            Trim = inner.Trim;
            Url = inner.Url;
            Value = inner.Value;
            Var = inner.Var;
        }
    }
}
