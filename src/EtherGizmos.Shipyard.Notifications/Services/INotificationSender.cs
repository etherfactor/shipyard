namespace EtherGizmos.Shipyard.Notifications.Services;

public interface INotificationSender<in TEvent>
{
    Task NotifyAsync(TEvent notification, CancellationToken cancellationToken = default);
}
