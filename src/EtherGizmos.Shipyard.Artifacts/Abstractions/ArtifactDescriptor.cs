namespace EtherGizmos.Shipyard.Abstractions;

public record ArtifactDescriptor(
    ArtifactUri Uri,
    string ContentType,
    string FileName,
    long Bytes
);
