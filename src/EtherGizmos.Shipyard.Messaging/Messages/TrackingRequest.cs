namespace EtherGizmos.Shipyard.Messages;

public record TrackingRequest
{
    public int PackageId { get; init; }

    public string CarrierSlug { get; init; } = null!;

    public string TrackingNumber { get; init; } = null!;
}
