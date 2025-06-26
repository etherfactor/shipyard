namespace EtherGizmos.Common.Utilities.Configuration;

public interface IConnectionResolver
{
    OneOfDatabaseConnection GetDatabaseConnection(string connectionId);
    
    OneOfEmailConnection GetEmailConnection(string connectionId);
}
