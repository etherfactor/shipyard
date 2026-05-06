namespace EtherGizmos.Shipyard.Abstractions;

public interface IBootstrapper
{
    int Order { get; }

    Task ExecuteAsync(
        CancellationToken cancellationToken  = default);
}
