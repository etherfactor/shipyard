using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Configuration;

public class DatabaseReferenceOptions
{
    [Required]
    public string ConnectionId { get; set; } = null!;
}
