using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Messaging.Configuration;
using EtherGizmos.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        @this.TryAddSingleton<IMessageBus, MessageBus>();
        @this.TryAddSingleton<IMessageReceiver, MessageReceiver>();
        @this.TryAddSingleton<IMessageSender, MessageSender>();

        @this.TryAddSingleton<IMessageSerializer, JsonMessageSerializer>();

        return new MessagingBuilder(@this);
    }
}
