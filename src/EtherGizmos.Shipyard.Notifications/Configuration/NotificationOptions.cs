namespace EtherGizmos.Shipyard.Notifications.Configuration;

public class NotificationOptions
{
    public bool IsEnabled { get; set; }

    public EmailNotificationOptions Email { get; set; } = new();
}
