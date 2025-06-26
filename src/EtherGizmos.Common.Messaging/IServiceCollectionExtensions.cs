using EtherGizmos.Common.Messaging.Abstractions;
using EtherGizmos.Common.Messaging.Configuration;
using EtherGizmos.Common.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EtherGizmos.Common.Messaging;

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
