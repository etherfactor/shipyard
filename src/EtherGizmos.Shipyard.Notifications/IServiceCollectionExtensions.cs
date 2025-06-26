using EtherGizmos.Common.Utilities.Configuration;
using EtherGizmos.Shipyard.Notifications.Configuration;
using EtherGizmos.Shipyard.Notifications.Models;
using EtherGizmos.Shipyard.Notifications.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EtherGizmos.Shipyard.Notifications;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNotifications(
        this IServiceCollection @this)
    {
        @this.AddScoped<SmtpNotificationSender>()
            .AddScoped<IEmailNotificationSender>(services =>
            {
                var notifOptions = services.GetRequiredService<IOptions<NotificationOptions>>()
                    .Value;

                var connectionId = notifOptions.Email.ConnectionId;

                var resolver = services.GetRequiredService<IConnectionResolver>();
                var connection = resolver.GetEmailConnection(connectionId);

                return connection.Match(
                    _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid email connection."),
                    smtp => services.GetRequiredService<SmtpNotificationSender>()
                );
            })
            .AddScoped(services =>
            {
                var notifOptions = services.GetRequiredService<IOptions<NotificationOptions>>()
                    .Value;

                var connectionId = notifOptions.Email.ConnectionId;

                var resolver = services.GetRequiredService<IConnectionResolver>();
                var connection = resolver.GetEmailConnection(connectionId);

                var smtp = connection.AsT1;

                var client = new SmtpClient();
                client.Connect(smtp.Host, smtp.Port, smtp.UseTls);
                client.Authenticate(smtp.Username, smtp.Password);

                return client;
            });

        @this.AddScoped<INotificationRenderer<PackageOutForDeliveryEvent>, PackageOutForDeliveryRenderer>();
        @this.AddScoped<ISmtpNotificationRenderer<PackageOutForDeliveryEvent>>(provider => provider.GetRequiredService<INotificationRenderer<PackageOutForDeliveryEvent>>());

        @this.AddScoped<INotificationRenderer<PackageDeliveredEvent>, PackageDeliveredRenderer>();
        @this.AddScoped<ISmtpNotificationRenderer<PackageDeliveredEvent>>(provider => provider.GetRequiredService<INotificationRenderer<PackageDeliveredEvent>>());

        return @this;
    }
}
