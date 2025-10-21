namespace EtherGizmos.Shipyard.Abstractions;

public record ArtifactDescriptor(
    ArtifactUri Uri,
    ArtifactType Type,
    long Bytes)
{
    public string ContentType => Type.ToContentType();
}
