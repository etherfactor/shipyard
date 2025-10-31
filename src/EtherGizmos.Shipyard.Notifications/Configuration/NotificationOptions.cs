namespace EtherGizmos.Shipyard.Configuration;

public class NotificationOptions
{
    public bool IsEnabled { get; set; }

    public EmailNotificationOptions Email { get; set; } = new();
}
