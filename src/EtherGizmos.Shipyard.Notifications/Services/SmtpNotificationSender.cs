using EtherGizmos.Shipyard.Notifications.Configuration;
using EtherGizmos.Shipyard.Notifications.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EtherGizmos.Shipyard.Notifications.Services;

internal class SmtpNotificationSender : IEmailNotificationSender
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<NotificationOptions> _notificationOptions;
    private readonly SmtpClient _smtpClient;

    public SmtpNotificationSender(
        IServiceProvider serviceProvider,
        IOptionsMonitor<NotificationOptions> notificationOptions,
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

        var renderer = (INotificationRenderer)_serviceProvider
            .GetRequiredService(typeof(ISmtpNotificationRenderer<>).MakeGenericType(notification.GetType()));

        var message = (MimeMessage)await renderer.RenderAsync(notification, cancellationToken);

        message.Sender = new MailboxAddress("Shipyard Notifications", options.Email.From);

        foreach (var recipient in options.Email.To)
        {
            message.To.Add(new MailboxAddress("", recipient));
        }

        foreach (var recipient in options.Email.Cc)
        {
            message.Cc.Add(new MailboxAddress("", recipient));
        }

        foreach (var recipient in options.Email.Bcc)
        {
            message.Bcc.Add(new MailboxAddress("", recipient));
        }

        await _smtpClient.SendAsync(message, cancellationToken);
    }
}
