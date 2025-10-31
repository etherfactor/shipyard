using EtherGizmos.Common.Utilities.Configuration;

namespace EtherGizmos.Common.Configuration;

public interface IConnectionResolver
{
    OneOfDatabaseConnection GetDatabaseConnection(string connectionId);

    OneOfEmailConnection GetEmailConnection(string connectionId);
}
