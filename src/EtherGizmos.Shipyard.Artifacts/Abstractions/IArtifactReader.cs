namespace EtherGizmos.Shipyard.Abstractions;

public interface IArtifactReader
{
    Task<(string ContentType, Stream Stream)> ReadAsync(
        ArtifactUri identifier,
        CancellationToken cancellationToken = default);
}
