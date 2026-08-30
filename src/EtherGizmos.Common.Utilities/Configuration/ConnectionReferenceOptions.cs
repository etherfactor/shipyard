using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class ConnectionReferenceOptions
{
    [Required]
    public string ConnectionId { get; set; } = null!;
}
