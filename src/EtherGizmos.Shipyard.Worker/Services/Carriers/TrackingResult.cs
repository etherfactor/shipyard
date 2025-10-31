namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public record TrackingResult
{
    public required string TrackingNumber { get; init; }

    public int LastStatusTypeId => Details.OrderBy(e => e.OccurredAt).Last().StatusTypeId;

    public required DateTimeOffset? EstimatedDeliveryAt { get; init; }

    public required IReadOnlyList<TrackingResultDetail> Details { get; init; }

    public required IReadOnlyList<TrackingResultArtifact> Artifacts { get; init; }
}
