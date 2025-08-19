using EtherGizmos.Common.Configuration;
using EtherGizmos.Common.Utilities.Configuration;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using EtherGizmos.Shipyard.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace EtherGizmos.Shipyard;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNotifications(
        this IServiceCollection @this,
        params Assembly[] eventAssemblies)
    {
        //For each provided assembly, we need to add its services
        foreach (var assembly in eventAssemblies)
        {
            //Register event renderers via reflection
            var renderers = assembly.GetTypes()
                .Where(type => type.GetInterfaces()
                    .Any(@interface => @interface.IsGenericType
                        && @interface.GetGenericTypeDefinition() == typeof(INotificationRenderer<>)))
                .ToList();

            foreach (var renderer in renderers)
            {
                var interfaces = renderer.GetInterfaces()
                    .Where(@interface => @interface.IsGenericType
                        && @interface.GetGenericTypeDefinition() == typeof(INotificationRenderer<>))
                    .ToList();

                foreach (var @interface in interfaces)
                {
                    @this.AddScoped(@interface, renderer);
                    @this.AddScoped(typeof(ISmtpNotificationRenderer<>)
                        .MakeGenericType(@interface.GenericTypeArguments), provider => provider.GetRequiredService(@interface));
                }
            }

            //Register event senders via reflection
            var events = assembly.GetTypes()
                .Where(type => type.IsAssignableTo(typeof(NotificationEvent))
                    && !type.IsAbstract)
                .ToList();

            foreach (var @event in events)
            {
                //First, register all of the physical classes
                var smtpSender = typeof(SmtpNotificationSender<>).MakeGenericType(@event);
                @this.AddScoped(smtpSender);

                //Then, build a factory that returns a different implementation depending on the connection
                var iemailSender = typeof(IEmailNotificationSender<>).MakeGenericType(@event);
                @this
                    .AddScoped(iemailSender, services =>
                    {
                        var (connectionId, connection) = services.GetEmailConnection();

                        return connection.Match(
                            _ => throw new InvalidOperationException($"The connection {connectionId} is not a valid email connection."),
                            smtp => services.GetRequiredService(smtpSender)
                        );
                    });
            }
        }

        //Register sender dependencies
        @this
            .AddTransient(services =>
            {
                var (_, connection) = services.GetEmailConnection();

                var smtp = connection.AsT1;

                var client = new SmtpClient();
                client.Connect(smtp.Host, smtp.Port, smtp.UseTls);
                client.Authenticate(smtp.Username, smtp.Password);

                return client;
            });

        return @this;
    }

    private static (string Id, OneOfEmailConnection Connection) GetEmailConnection(
        this IServiceProvider @this)
    {
        var notifOptions = @this.GetRequiredService<IOptions<NotificationOptions>>()
            .Value;

        var connectionId = notifOptions.Email.ConnectionId;

        var resolver = @this.GetRequiredService<IConnectionResolver>();
        var connection = resolver.GetEmailConnection(connectionId);

        return (connectionId, connection);
    }
}
