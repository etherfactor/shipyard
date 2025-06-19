using FluentMigrator.Runner;

namespace EtherGizmos.Shipyard.Database.Services;

internal class MigrationManager : IMigrationManager
{
    private readonly IMigrationRunner _migrationRunner;

    private bool _isMigrated;
    private readonly object _lock = new();

    public MigrationManager(
        IMigrationRunner migrationRunner)
    {
        _migrationRunner = migrationRunner;
    }

    public void EnsureMigrated()
    {
        if (_isMigrated)
            return;

        lock (_lock)
        {
            if (_isMigrated)
                return;

            _migrationRunner.MigrateUp();
            _isMigrated = true;
        }
    }
}
