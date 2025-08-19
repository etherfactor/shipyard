namespace EtherGizmos.Shipyard.Abstractions;

public interface IMigrationManager
{
    Task EnsureMigratedAsync(CancellationToken cancellationToken = default);
}
