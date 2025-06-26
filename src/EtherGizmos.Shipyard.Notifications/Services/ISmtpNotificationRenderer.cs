using System.Net.Mail;

namespace EtherGizmos.Shipyard.Notifications.Services;

public interface ISmtpNotificationRenderer<in TEvent>
    where TEvent : class
{
    Task<MailMessage> RenderAsync(TEvent notification, CancellationToken cancellationToken = default);
}
