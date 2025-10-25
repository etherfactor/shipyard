namespace EtherGizmos.Shipyard.Abstractions;

public interface IArtifactWriter
{
    Task<ArtifactDescriptor> WriteAsync(
        string container,
        ArtifactFormat type,
        string fileName,
        Stream data,
        CancellationToken cancellationToken = default);
}

public static class IArtifactWriterExtensions
{
    public static async Task<ArtifactDescriptor> WriteForRunAsync(
        this IArtifactWriter @this,
        int runId,
        ArtifactFormat type,
        string fileName,
        Stream data,
        CancellationToken cancellationToken = default)
    {
        var container = $"runs/{runId}";
        return await @this.WriteAsync(container, type, fileName, data, cancellationToken);
    }
}
