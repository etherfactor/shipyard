using EtherGizmos.Common.Utilities.Configuration;
using EtherGizmos.Shipyard.Notifications.Configuration;
using EtherGizmos.Shipyard.Notifications.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EtherGizmos.Shipyard.Notifications;

public static class IServiceCollectionExtensions
{
public static IServiceCollection AddNotifications(
    this IServiceCollection @this)
{
    @this.AddScoped<SmtpNotificationSender>()
        .AddScoped<IEmailNotificationSender>(services =>
        {
            var emailOptions = services.GetRequiredService<IOptions<EmailNotificationOptions>>()
                .Value;

            var connectionId = emailOptions.ConnectionId;

            var resolver = services.GetRequiredService<IConnectionResolver>();
            var connection = resolver.GetEmailConnection(connectionId);

            return connection.Match(
                _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid email connection."),
                smtp => services.GetRequiredService<SmtpNotificationSender>()
            );
        })
        .AddScoped(services =>
        {
            var emailOptions = services.GetRequiredService<IOptions<EmailNotificationOptions>>()
                .Value;

            var connectionId = emailOptions.ConnectionId;

            var resolver = services.GetRequiredService<IConnectionResolver>();
            var connection = resolver.GetEmailConnection(connectionId);

            var smtp = connection.AsT1;

            return new SmtpClient()
            {
                Host = smtp.Host,
                Port = smtp.Port,
                EnableSsl = smtp.UseTls,
                Credentials = new NetworkCredential(smtp.Username, smtp.Password),
            };
        });

    return @this;
}
}
