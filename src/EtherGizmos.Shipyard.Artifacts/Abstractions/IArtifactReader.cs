namespace EtherGizmos.Shipyard.Abstractions;

public interface IArtifactReader
{
    Task<(ArtifactType Type, Stream Stream)> ReadAsync(
        ArtifactUri identifier,
        CancellationToken cancellationToken = default);
}
