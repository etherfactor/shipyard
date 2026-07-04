#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public record PackageReturnedEvent : ShipyardEvent
{
    public required int PackageId { get; init; }

    public required int CarrierId { get; init; }

    public required string CarrierName { get; init; }

    public required string TrackingNumber { get; init; }

    public required string? TrackingUrl { get; init; }

    public required string? Contents { get; init; }

    public required List<PackageReturnedEventUpdate> Updates { get; init; }
}

public record PackageReturnedEventUpdate
{
    public required string Status { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required string? Location { get; init; }

    public required string? Description { get; init; }
}
