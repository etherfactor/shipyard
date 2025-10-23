using EtherGizmos.Shipyard.Abstractions;

namespace EtherGizmos.Shipyard.Messages;

public record TrackingResponse
{
    public int ExecutionId { get; init; }

    public bool IsSuccess { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset CompletedAt { get; set; }

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
    public ArtifactUri Uri { get; init; }

    public string ContentType { get; init; } = null!;

    public long Bytes { get; init; }

    public short StepIndex { get; init; }
}
