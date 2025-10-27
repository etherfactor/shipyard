using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Utilities.Configuration;

public class PostgreSqlOptions : DatabaseConnectionOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;
}
