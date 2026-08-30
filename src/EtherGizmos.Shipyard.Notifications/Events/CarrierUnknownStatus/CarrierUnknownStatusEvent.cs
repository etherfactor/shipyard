#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public record CarrierUnknownStatusEvent : ShipyardEvent
{
    public required int CarrierId { get; init; }

    public required string CarrierName { get; init; }

    public required DateTimeOffset ObservedAt { get; init; }

    public required string? StatusText { get; init; }
}
