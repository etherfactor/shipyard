using EtherGizmos.Shipyard.Models;

namespace EtherGizmos.Shipyard.Services;

public interface INotificationRenderer<in TEvent> :
    INotificationRenderer,
    ISmtpNotificationRenderer<TEvent>
    where TEvent : NotificationEvent;

public interface INotificationRenderer
{
    Task<object> RenderAsync(NotificationEvent notification, CancellationToken cancellationToken = default);
}
