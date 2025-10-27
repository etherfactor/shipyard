using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Database.Configuration;

public class PostgreSqlOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;
}
