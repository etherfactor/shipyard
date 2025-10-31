namespace EtherGizmos.Shipyard.Messages;

public record PackageOutForDelivery
{
    public int PackageId { get; init; }

    public int CarrierId { get; init; }

    public string CarrierName { get; init; } = null!;

    public string TrackingNumber { get; init; } = null!;

    public string? Contents { get; init; }

    public DateTimeOffset OccurredAt { get; init; }

    public string? Location { get; init; }

    public string? Description { get; init; }

    public DateTimeOffset? EstimatedDeliveryAt { get; init; }
}
