using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Shipyard.Services;

internal class MigrationManager : IMigrationManager
{
    private readonly IServiceProvider _serviceProvider;

    private bool _isMigrated;
    private readonly object _lock = new();

    public MigrationManager(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void EnsureMigrated()
    {
        if (_isMigrated)
            return;

        lock (_lock)
        {
            if (_isMigrated)
                return;

            using var scope = _serviceProvider.CreateScope();
            var provider = scope.ServiceProvider;

            var runner = provider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
            _isMigrated = true;
        }
    }
}
