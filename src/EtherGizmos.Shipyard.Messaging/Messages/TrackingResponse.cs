namespace EtherGizmos.Shipyard.Messages;

public record TrackingResponse
{
    public int ExecutionId { get; init; }

    public int PackageId { get; init; }

    public DateTimeOffset? EstimatedDeliveryAt { get; init; }

    public IReadOnlyList<TrackingResponseDetail> Details { get; init; } = [];

    public IReadOnlyList<TrackingResponseArtifact> Artifacts { get; init; } = [];
}

public record TrackingResponseDetail
{
    public DateTimeOffset OccurredAt { get; init; }

    public int StatusTypeId { get; init; }

    public string? Location { get; init; }

    public string? Description { get; init; }
}

public record TrackingResponseArtifact
{
    public string ArtifactUri { get; init; } = null!;

    public short StepIndex { get; init; }
}
