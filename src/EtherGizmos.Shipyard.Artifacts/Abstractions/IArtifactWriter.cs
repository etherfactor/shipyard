namespace EtherGizmos.Shipyard.Abstractions;

public interface IArtifactWriter
{
    Task<ArtifactUri> WriteAsync(
        string container,
        ArtifactType type,
        string fileName,
        Stream data,
        CancellationToken cancellationToken = default);
}

public static class IArtifactWriterExtensions
{
    public static async Task<ArtifactUri> WriteForRunAsync(
        this IArtifactWriter @this,
        int runId,
        ArtifactType type,
        string fileName,
        Stream data,
        CancellationToken cancellationToken = default)
    {
        var container = $"runs/{runId}";
        return await @this.WriteAsync(container, type, fileName, data, cancellationToken);
    }
}
