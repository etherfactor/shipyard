using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Messaging.Configuration;

public class RabbitMQMessagingOptions
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    [Required]
    public string Host { get; set; } = null!;

    public int Port { get; set; } = 5672;
}
