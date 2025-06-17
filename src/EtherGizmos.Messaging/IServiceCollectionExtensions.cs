using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Messaging.Configuration;
using EtherGizmos.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Messaging;

public static class IServiceCollectionExtensions
{
    public static IMessagingBuilder AddMessaging(
        this IServiceCollection @this,
        Action<MessagingOptions, IConfiguration> configureOptions)
    {
        @this.AddOptions<MessagingOptions>()
            .Configure(configureOptions);

        @this.AddHostedService<MessagePumpHostedService>();

        @this.AddSingleton<IMessageBus, MessageBus>();
        @this.AddSingleton<IMessageReceiver, MessageReceiver>();
        @this.AddSingleton<IMessageSender, MessageSender>();

        return new MessagingBuilder(@this);
    }
}
