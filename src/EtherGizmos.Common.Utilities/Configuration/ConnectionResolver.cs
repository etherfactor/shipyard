using EtherGizmos.Common.Utilities.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtherGizmos.Common.Configuration;

internal class ConnectionResolver : IConnectionResolver
{
    private readonly IOptionsMonitor<Dictionary<string, ConnectionOptions>> _options;
    private readonly IConfiguration _configuration;

    public ConnectionResolver(
        IOptionsMonitor<Dictionary<string, ConnectionOptions>> options,
        IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;
    }

    public OneOfDatabaseConnection GetDatabaseConnection(
        string connectionId)
    {
        var connection = GetConnection<DatabaseConnectionOptions>(connectionId, ConnectionType.Database);
        var result = connection switch
        {
            PostgreSqlOptions postgreSql => (OneOfDatabaseConnection)postgreSql,
            _ => connection
        };

        return result;
    }

    public OneOfEmailConnection GetEmailConnection(
        string connectionId)
    {
        var connection = GetConnection<EmailConnectionOptions>(connectionId, ConnectionType.Email);
        var result = connection switch
        {
            SmtpOptions smtp => (OneOfEmailConnection)smtp,
            _ => connection
        };

        return result;
    }

    private TOptions GetConnection<TOptions>(
        string connectionId,
        ConnectionType expectedType)
        where TOptions : new()
    {
        var options = _options.CurrentValue;

        if (options.TryGetValue(connectionId, out var connection))
        {
            if (connection.Type == expectedType)
            {
                var properties = connection.GetType().GetProperties()
                    .Where(e => e.PropertyType.IsAssignableTo(typeof(TOptions)))
                    .Where(e => e.GetValue(connection) is not null)
                    .ToList();

                if (properties.Count == 1)
                {
                    return (TOptions)properties.Single().GetValue(connection)!;
                }
            }
        }

        return new TOptions();
    }
}
