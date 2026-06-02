using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace EtherGizmos.Shipyard.Services.Health;

internal class DatabaseHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<ConnectionReferenceOptions> _dbOptions;
    private readonly IConnectionResolver _connectionResolver;

    public DatabaseHealthCheck(
        IOptionsMonitor<ConnectionReferenceOptions> dbOptions,
        IConnectionResolver connectionResolver)
    {
        _dbOptions = dbOptions;
        _connectionResolver = connectionResolver;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var dbOptions = _dbOptions.Get("Database");

        try
        {
            using var dbConnection = _connectionResolver.CreateDbConnection(dbOptions.ConnectionId);

            await dbConnection.OpenAsync(cancellationToken);

            using var command = dbConnection.CreateCommand();
            command.CommandText = "select 1;";

            await command.ExecuteNonQueryAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
