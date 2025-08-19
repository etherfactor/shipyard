namespace EtherGizmos.Shipyard.Services;

public interface INotificationSender<in TEvent>
{
    Task NotifyAsync(TEvent notification, CancellationToken cancellationToken = default);
}
