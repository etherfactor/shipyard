using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Messaging.Configuration;

public class MessagePublisherOptions
{
    public bool IsTopic { get; set; } = false;

    [Required]
    public string Name { get; set; } = null!;
}
