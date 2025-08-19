using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Utilities.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace EtherGizmos.Common.Configuration;

internal class CertificateResolver : ICertificateResolver
{
    private readonly IOptionsMonitor<Dictionary<string, CertificateOptions>> _options;
    private readonly IConfiguration _configuration;

    public CertificateResolver(
        IOptionsMonitor<Dictionary<string, CertificateOptions>> options,
        IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;
    }

    public X509Certificate2 GetCertificate(string certificateId)
    {
        var connection = GetConnection<CertificateReferenceOptions>(certificateId);
        var result = connection switch
        {
            PostgreSqlOptions postgreSql => (OneOfDatabaseConnection)postgreSql,
            _ => connection
        };

        return result;
    }

    private TOptions GetConnection<TOptions>(
        string connectionId)
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
