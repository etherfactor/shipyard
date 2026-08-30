using System.Collections.Immutable;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public record PackageEtaChangedEvent : ShipyardEvent
{
    public required int PackageId { get; init; }

    public required int CarrierId { get; init; }

    public required string CarrierName { get; init; }

    public required string TrackingNumber { get; init; }

    public required string? TrackingUrl { get; init; }

    public required string? Contents { get; init; }

    public DateTimeOffset? PreviousEta { get; set; }

    public DateTimeOffset? CurrentEta { get; set; }

    public required ImmutableList<PackageEtaChangedEventUpdate> Updates { get; init; }
}

public record PackageEtaChangedEventUpdate
{
    public required string Status { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }

    public required string? Location { get; init; }

    public required string? Description { get; init; }
}
