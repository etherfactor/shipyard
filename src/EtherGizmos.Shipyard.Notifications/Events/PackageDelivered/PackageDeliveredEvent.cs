#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public record PackageDeliveredEvent : ShipyardEvent
{
    public required int PackageId { get; init; }

    public required int CarrierId { get; init; }

    public required string CarrierName { get; init; }

    public required string TrackingNumber { get; init; }

    public required string? Contents { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required List<PackageDeliveredEventUpdate> Updates { get; init; }
}

public record PackageDeliveredEventUpdate
{
    public required string Status { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required string? Location { get; init; }

    public required string? Details { get; init; }
}