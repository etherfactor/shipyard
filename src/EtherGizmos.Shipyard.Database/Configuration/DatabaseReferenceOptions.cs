using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Database.Configuration;

public class DatabaseReferenceOptions
{
    [Required]
    public string ConnectionId { get; set; } = null!;
}
