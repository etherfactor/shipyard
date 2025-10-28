namespace EtherGizmos.Shipyard.Abstractions;

public record ArtifactRead(
    ArtifactUri Uri,
    string FileName,
    string ContentType,
    long Bytes,
    Stream Stream
);
