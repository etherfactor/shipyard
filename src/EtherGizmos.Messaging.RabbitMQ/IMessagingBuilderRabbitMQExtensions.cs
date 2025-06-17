using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Messaging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EtherGizmos.Messaging;

public static class IMessagingBuilderRabbitMQExtensions
{
    public static IMessagingBuilder UseRabbitMQ(
        this IMessagingBuilder @this)
    {
        @this.Services
            .AddKeyedSingleton(RabbitMQConstants.MessagingKey, (provider, _) =>
            {
                var options = provider
                    .GetRequiredService<IOptionsSnapshot<RabbitMQMessagingOptions>>()
                    .Value;

                var factory = new ConnectionFactory()
                {
                    HostName = options.Host,
                    UserName = options.Username!,
                    Password = options.Password!,
                    Port = options.Port,
                };

                return factory;
            });

        return @this;
    }
}
