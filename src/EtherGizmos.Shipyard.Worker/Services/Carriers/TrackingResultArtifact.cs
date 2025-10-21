namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

public record TrackingResultArtifact
{
    public string ArtifactUri { get; init; } = null!;

    public short StepIndex { get; init; }
}
