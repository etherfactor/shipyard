using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Services.Carriers;

public record TrackingResultArtifact
{
    public ArtifactUri Uri { get; init; }

    public string ContentType { get; init; } = null!;

    public string FileName { get; set; } = null!;

    public long Bytes { get; init; }

    public short? StepIndex { get; init; }
}
