using EtherGizmos.Common.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Channels;

namespace EtherGizmos.Common.Services;

internal class RabbitMQListener : IMessageListener
{
    private readonly ILogger _logger;
    private readonly ConnectionFactory _rmqConnectionFactory;
    private readonly string? _queue;
    private readonly string? _topic;
    private readonly string? _subscription;

    private readonly Channel<ReceivedMessage> _channel = System.Threading.Channels.Channel.CreateUnbounded<ReceivedMessage>();

    private IConnection? _rmqConnection;
    private IChannel? _rmqChannel;
    private AsyncEventingBasicConsumer? _rmqConsumer;

    private bool _disposed;

    public ChannelReader<ReceivedMessage> Channel => _channel;

    public RabbitMQListener(
        IServiceProvider serviceProvider,
        string queue)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQListener>>();
        _rmqConnectionFactory = serviceProvider.GetRequiredKeyedService<ConnectionFactory>(RabbitMQConstants.MessagingKey);
        _queue = queue;
    }

    public RabbitMQListener(
        IServiceProvider serviceProvider,
        string topic,
        string subscription)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQListener>>();
        _rmqConnectionFactory = serviceProvider.GetRequiredKeyedService<ConnectionFactory>(RabbitMQConstants.MessagingKey);
        _topic = topic;
        _subscription = subscription;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _rmqConnection = await _rmqConnectionFactory.CreateConnectionAsync(cancellationToken: cancellationToken);
        _rmqChannel = await _rmqConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        if (_queue is not null)
        {
            await _rmqChannel.QueueDeclareAsync(_queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        }
        else
        {
            await _rmqChannel.ExchangeDeclareAsync(_topic!, ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: cancellationToken);
            await _rmqChannel.QueueDeclareAsync($"{_topic}:{_subscription}", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
            await _rmqChannel.QueueBindAsync($"{_topic}:{_subscription}", exchange: _topic!, routingKey: _subscription!, cancellationToken: cancellationToken);
        }

        _rmqConsumer = new AsyncEventingBasicConsumer(_rmqChannel);
        _rmqConsumer.ReceivedAsync += RmqConsumer_ReceivedAsync;

        await _rmqChannel.BasicConsumeAsync(_queue ?? $"{_topic}:{_subscription}", autoAck: false, consumer: _rmqConsumer, cancellationToken: cancellationToken);
    }

    private async Task RmqConsumer_ReceivedAsync(
        object sender, BasicDeliverEventArgs @event)
    {
        while (_rmqChannel is null)
        {
            await Task.Delay(200, @event.CancellationToken);
        }

        try
        {
            var body = Encoding.UTF8.GetString(@event.Body.Span);

            var allHeaders = @event.BasicProperties.Headers?
                .Select(e => new KeyValuePair<string, string>(e.Key, Encoding.UTF8.GetString((byte[])e.Value!) ?? ""))
                .ToDictionary() ?? new Dictionary<string, string>();

            var headers = allHeaders
                .Where(e => e.Key != "$type")
                .Where(e => e.Key != "$logical")
                .ToImmutableDictionary();

            var actions = new RabbitMQMessageActions(_rmqChannel, @event.DeliveryTag);

            var message = new ReceivedMessage()
            {
                Id = @event.DeliveryTag.ToString(),
                Type = allHeaders["$type"],
                Body = body,
                Headers = headers,
                LogicalSourceName = allHeaders["$logical"],
                Actions = actions,
            };

            await _channel.Writer.WriteAsync(message, @event.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encountered an error while receiving a RabbitMQ message");
            await _rmqChannel.BasicNackAsync(@event.DeliveryTag, false, true, @event.CancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_rmqChannel is not null)
            await _rmqChannel.DisposeAsync();

        if (_rmqConnection is not null)
            await _rmqConnection.DisposeAsync();

        if (_rmqConsumer is not null)
        {
            _rmqConsumer.ReceivedAsync -= RmqConsumer_ReceivedAsync;
        }

        _channel.Writer.Complete();
        await _channel.Reader.Completion;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
