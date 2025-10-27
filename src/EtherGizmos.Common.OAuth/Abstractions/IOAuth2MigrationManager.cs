namespace EtherGizmos.Common.Abstractions;

public interface IOAuth2MigrationManager
{
    Task EnsureMigratedAsync(CancellationToken cancellationToken = default);
}
