namespace EtherGizmos.Shipyard.Abstractions;

public interface IArtifactReader
{
    Task<ArtifactRead> ReadAsync(
        ArtifactUri identifier,
        CancellationToken cancellationToken = default);
}
