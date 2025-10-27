using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Messaging.Configuration;
using EtherGizmos.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace EtherGizmos.Messaging;

public static class IMessagingBuilderRabbitMQExtensions
{
    public static IMessagingBuilder UseRabbitMQ(
        this IMessagingBuilder @this,
        Action<RabbitMQMessagingOptions, IConfiguration> configureOptions)
    {
        @this.Services
            .AddOptions<RabbitMQMessagingOptions>()
            .Configure(configureOptions);

        @this.Services
            .AddSingleton<RabbitMQTransport>()
            .AddSingleton<IMessagePublisherFactory>(e => e.GetRequiredService<RabbitMQTransport>())
            .AddSingleton<IMessageListenerFactory>(e => e.GetRequiredService<RabbitMQTransport>());

        @this.Services
            .AddKeyedSingleton(RabbitMQConstants.MessagingKey, (provider, _) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<RabbitMQMessagingOptions>>()
                    .Value;

                var factory = new ConnectionFactory();
                if (options.ConnectionString is not null)
                {
                    factory.Uri = new Uri(options.ConnectionString);
                }
                else
                {
                    factory.HostName = options.Host ?? "localhost";
                    factory.UserName = options.Username!;
                    factory.Password = options.Password!;
                    factory.Port = options.Port;
                }

                return factory;
            });

        return @this;
    }
}
