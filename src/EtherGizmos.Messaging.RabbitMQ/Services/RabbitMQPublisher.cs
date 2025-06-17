using EtherGizmos.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace EtherGizmos.Messaging.Services;

internal class RabbitMQPublisher : IMessagePublisher
{
    private readonly ILogger _logger;
    private readonly ConnectionFactory _rmqConnectionFactory;
    private readonly string? _queue;
    private readonly string? _topic;

    private readonly Channel<SentMessage> _channel = System.Threading.Channels.Channel.CreateUnbounded<SentMessage>();

    private IConnection? _rmqConnection;
    private IChannel? _rmqChannel;
    private CancellationTokenSource? _publishCts;
    private Task? _publishTask;

    private bool _disposed;

    public ChannelWriter<SentMessage> Channel => _channel;

    public RabbitMQPublisher(
        IServiceProvider serviceProvider,
        string queue)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQPublisher>>();
        _rmqConnectionFactory = serviceProvider.GetRequiredKeyedService<ConnectionFactory>(RabbitMQConstants.MessagingKey);
        _queue = queue;
    }

    public RabbitMQPublisher(
        IServiceProvider serviceProvider,
        string topic,
        string subscription)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<RabbitMQPublisher>>();
        _rmqConnectionFactory = serviceProvider.GetRequiredKeyedService<ConnectionFactory>(RabbitMQConstants.MessagingKey);
        _topic = topic;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken = default)
    {
        _rmqConnection = await _rmqConnectionFactory.CreateConnectionAsync(cancellationToken: cancellationToken);
        _rmqChannel = await _rmqConnection.CreateChannelAsync(cancellationToken: cancellationToken);

        if (_queue is not null)
        {
            await _rmqChannel.QueueDeclareAsync(_queue, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
        }
        else
        {
            await _rmqChannel.ExchangeDeclareAsync(_topic!, ExchangeType.Topic, durable: true, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
        }

        _publishCts = new();
        _publishTask = ExecuteLoopAsync(_publishCts.Token);
    }

    public async Task StopAsync(
        CancellationToken cancellationToken = default)
    {
        _publishCts?.Cancel();
        if (_publishTask is not null)
            await _publishTask;

        _channel.Writer.Complete();
        await _channel.Reader.Completion;

        if (_rmqChannel is not null)
            await _rmqChannel.DisposeAsync();

        if (_rmqConnection is not null)
            await _rmqConnection.DisposeAsync();
    }

    private async Task ExecuteLoopAsync(
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_rmqChannel is null)
            {
                await Task.Delay(100, cancellationToken);
                continue;
            }

            try
            {
                await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
                {
                    var properties = new BasicProperties()
                    {
                        Headers = message.Headers.Select(e => new KeyValuePair<string, object?>(e.Key, e.Value)).ToDictionary(),
                    };

                    var bytes = Encoding.UTF8.GetBytes(message.Body);
                    await _rmqChannel.BasicPublishAsync(
                        exchange: _topic ?? _queue!, routingKey: string.Empty, mandatory: true, body: bytes,
                        basicProperties: properties, cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encountered an error during the RabbitMQ publish loop");
                await Task.Delay(2000, cancellationToken);
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                StopAsync().Wait();
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
