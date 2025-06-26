namespace EtherGizmos.Common.Utilities.Configuration;

public class NotificationOptions
{
    public bool IsEnabled { get; set; }

    public SmtpOptions Smtp { get; set; } = new();
}
