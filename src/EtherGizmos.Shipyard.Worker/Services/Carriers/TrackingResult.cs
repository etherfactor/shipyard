namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public record TrackingResult
{
    public required string TrackingNumber { get; init; }

    public int LastStatusTypeId => Details.OrderBy(e => e.EventOccurredAt).Last().StatusTypeId;

    public required DateTimeOffset? EstimatedDeliveryAt { get; init; }

    public required IReadOnlyList<TrackingResultDetail> Details { get; init; }
}
