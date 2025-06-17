using EtherGizmos.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Messaging;

public static class IMessagingBuilderExtensions
{
    public static IMessagingBuilder AddMiddleware<TMiddleware>(
        this IMessagingBuilder @this,
        string? logicalName = null)
        where TMiddleware : class, IMessageMiddleware
    {
        if (logicalName is not null)
        {
            @this.Services.AddKeyedSingleton<IMessageMiddleware, TMiddleware>(logicalName);
        }
        else
        {
            @this.Services.AddSingleton<IMessageMiddleware, TMiddleware>();
        }

        return @this;
    }

    public static IMessagingBuilder AddTransformer<TTransformer>(
        this IMessagingBuilder @this,
        string? logicalName = null)
        where TTransformer : class, IMessageTransformer
    {
        if (logicalName is not null)
        {
            @this.Services.AddKeyedSingleton<IMessageTransformer, TTransformer>(logicalName);
        }
        else
        {
            @this.Services.AddSingleton<IMessageTransformer, TTransformer>();
        }

        return @this;
    }

    public static IMessagingBuilder UseSerializer<TSerializer>(
        this IMessagingBuilder @this,
        string? logicalName = null)
        where TSerializer : class, IMessageSerializer
    {
        if (logicalName is not null)
        {
            @this.Services.AddKeyedSingleton<IMessageSerializer, TSerializer>(logicalName);
        }
        else
        {
            @this.Services.AddSingleton<IMessageSerializer, TSerializer>();
        }

        return @this;
    }
}
