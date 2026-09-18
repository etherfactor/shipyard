namespace EtherGizmos.Shipyard.Abstractions;

public interface IArtifactSender
{
    Task SendAsync(
        int executionId,
        string contentType,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default);
}
