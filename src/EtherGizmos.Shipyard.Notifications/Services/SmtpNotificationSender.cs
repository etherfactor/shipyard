using EtherGizmos.Shipyard.Notifications.Configuration;
using EtherGizmos.Shipyard.Notifications.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace EtherGizmos.Shipyard.Notifications.Services;

internal class SmtpNotificationSender : IEmailNotificationSender
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<EmailNotificationOptions> _notificationOptions;
    private readonly SmtpClient _smtpClient;

    public SmtpNotificationSender(
        IServiceProvider serviceProvider,
        IOptionsMonitor<EmailNotificationOptions> notificationOptions,
        SmtpClient smtpClient)
    {
        _serviceProvider = serviceProvider;
        _notificationOptions = notificationOptions;
        _smtpClient = smtpClient;
    }

    public async Task NotifyAsync(
        NotificationEvent notification,
        CancellationToken cancellationToken = default)
    {
        var options = _notificationOptions.CurrentValue;

        var renderer = (ISmtpNotificationRenderer<NotificationEvent>)_serviceProvider
            .GetRequiredService(typeof(ISmtpNotificationRenderer<>).MakeGenericType(notification.GetType()));

        var message = await renderer.RenderAsync(notification, cancellationToken);

        foreach (var recipient in options.To)
        {
            message.To.Add(new MailAddress(recipient));
        }

        foreach (var recipient in options.Cc)
        {
            message.CC.Add(new MailAddress(recipient));
        }

        foreach (var recipient in options.Bcc)
        {
            message.Bcc.Add(new MailAddress(recipient));
        }

        await _smtpClient.SendMailAsync(message, cancellationToken);
    }
}
