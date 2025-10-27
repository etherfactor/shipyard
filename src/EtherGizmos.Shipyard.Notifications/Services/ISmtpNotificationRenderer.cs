using EtherGizmos.Shipyard.Models;
using MimeKit;

namespace EtherGizmos.Shipyard.Services;

public interface ISmtpNotificationRenderer<in TEvent>
    where TEvent : NotificationEvent
{
    Task<MimeMessage> RenderAsync(TEvent notification, CancellationToken cancellationToken = default);
}
