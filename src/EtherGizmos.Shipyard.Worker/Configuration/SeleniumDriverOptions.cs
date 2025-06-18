using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Configuration;

public class SeleniumDriverOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;
}
