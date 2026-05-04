using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;

namespace EtherGizmos.Shipyard.Api.Services.Health;

internal class DatabaseHealthCheck : IHealthCheck
{
    private readonly IOptions<DatabaseReferenceOptions> _dbOptions;
    private readonly IConnectionResolver _connectionResolver;

    public DatabaseHealthCheck(
        IOptions<DatabaseReferenceOptions> dbOptions,
        IConnectionResolver connectionResolver)
    {
        _dbOptions = dbOptions;
        _connectionResolver = connectionResolver;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var dbOptions = _dbOptions.Value;

        try
        {
            var dbConnection = _connectionResolver.CreateDbConnection(dbOptions.ConnectionId);

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
