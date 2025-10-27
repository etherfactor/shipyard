using EtherGizmos.Shipyard.Notifications.Models;

namespace EtherGizmos.Shipyard.Notifications.Services;

public interface INotificationSender<in TEvent>
{
    Task NotifyAsync(NotificationEvent notification, CancellationToken cancellationToken = default);
}
