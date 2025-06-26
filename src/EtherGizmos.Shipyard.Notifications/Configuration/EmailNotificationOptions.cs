using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Notifications.Configuration;

public class EmailNotificationOptions
{
    public bool IsEnabled { get; set; }

    [Required]
    public string ConnectionId { get; set; } = null!;

    [Required]
    public string From { get; set; } = null!;

    public List<string> To { get; set; } = [];

    public List<string> Cc { get; set; } = [];

    public List<string> Bcc { get; set; } = [];
}
