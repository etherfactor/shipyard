using EtherGizmos.Messaging.Abstractions;
using EtherGizmos.Messaging.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace EtherGizmos.Messaging.Services;

internal class MessageReceiver : IMessageReceiver
{
    private readonly MessagingOptions _options;

    public IServiceProvider Services { get; }

    public MessageReceiver(
        IServiceProvider services,
        IOptions<MessagingOptions> options)
    {
        _options = options.Value;

        Services = services;
    }

    public async Task ReceiveAsync(
        ReceivedMessage message,
        CancellationToken cancellationToken = default)
    {
        var type = _options.ConvertType(message.Type);

        var method = typeof(MessageReceiver)
            .GetMethod(nameof(ReceiveInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(type);

        var result = (Task)method.Invoke(this, [message, cancellationToken])!;
        await result;
    }

    private async Task ReceiveInternalAsync<TMessage>(
        ReceivedMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class, new()
    {
        var logicalName = message.LogicalSourceName;

        var transformers = Services.GetRequiredService<IEnumerable<IMessageTransformer>>()
            .Concat(Services.GetRequiredKeyedService<IEnumerable<IMessageTransformer>>(logicalName))
            .Reverse();

        foreach (var transformer in transformers)
        {
            message = await transformer.UnwrapAsync(message, cancellationToken);
        }

        var serializer = Services.GetService<IMessageSerializer>()
            ?? Services.GetRequiredKeyedService<IMessageSerializer>(logicalName);

        var deserialized = serializer.Deserialize<TMessage>(message.Body);

        var middleware = Services.GetRequiredService<IEnumerable<IMessageMiddleware>>()
            .Concat(Services.GetRequiredKeyedService<IEnumerable<IMessageMiddleware>>(logicalName));

        var consumers = Services.GetRequiredService<IEnumerable<IMessageConsumer<TMessage>>>();

        if (!consumers.Any())
        {
            throw new InvalidOperationException($"No consumers available for type {typeof(TMessage)}");
        }

        var context = new MessageContext<TMessage>(deserialized, message.Actions, cancellationToken);
        var execute = async () =>
        {
            await Parallel.ForEachAsync(consumers, async (consumer, cancellationToken) =>
            {
                await consumer.ConsumeAsync(context);
            });
        };

        var pipeline = middleware.Reverse().Aggregate(execute, (acc, middleware) =>
        {
            return () => middleware.InvokeAsync(message, acc);
        });

        await pipeline();
    }
}
