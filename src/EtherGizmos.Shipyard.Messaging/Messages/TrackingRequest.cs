namespace EtherGizmos.Shipyard.Messages;

public record TrackingRequest
{
    public int ExecutionId { get; init; }

    public int PackageId { get; init; }

    public int CarrierId { get; init; }

    public string TrackingNumber { get; init; } = null!;
}
