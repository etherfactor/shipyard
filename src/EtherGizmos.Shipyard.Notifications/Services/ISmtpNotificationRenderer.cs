using EtherGizmos.Shipyard.Notifications.Models;
using MimeKit;

namespace EtherGizmos.Shipyard.Notifications.Services;

public interface ISmtpNotificationRenderer<in TEvent>
    where TEvent : NotificationEvent
{
    Task<MimeMessage> RenderAsync(TEvent notification, CancellationToken cancellationToken = default);
}
