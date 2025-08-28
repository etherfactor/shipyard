namespace EtherGizmos.Shipyard.Messages;

public record TrackingResponse
{
    public int PackageId { get; init; }

    public DateTimeOffset? EstimatedDeliveryAt { get; init; }

    public IReadOnlyList<TrackingResponseDetail> Details { get; init; } = [];
}

public record TrackingResponseDetail
{
    public DateTimeOffset OccurredAt { get; init; }

    public int StatusTypeId { get; init; }

    public string? Location { get; init; }

    public string? Description { get; init; }
}
