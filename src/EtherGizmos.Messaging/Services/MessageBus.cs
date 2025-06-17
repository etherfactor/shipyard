using EtherGizmos.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow;

namespace EtherGizmos.Messaging.Services;

internal class MessageBus : IMessageBus
{
    private readonly ILogger _logger;
    private readonly IMessageListenerFactory _listenerFactory;
    private readonly IMessagePublisherFactory _publisherFactory;
    private readonly IMessageReceiver _receiver;

    private readonly ConcurrentDictionary<string, Lazy<Task<(IMessageListener Listener, CancellationTokenSource Cts)>>> _listeners = [];
    private readonly ConcurrentDictionary<string, Lazy<Task<IMessagePublisher>>> _publishers = [];

    private bool _isRunning = false;
    private ActionBlock<ReceivedMessage>? _pump;

    public MessageBus(
        ILogger logger,
        IMessageListenerFactory listenerFactory,
        IMessagePublisherFactory publisherFactory,
        IMessageReceiver receiver)
    {
        _logger = logger;
        _listenerFactory = listenerFactory;
        _publisherFactory = publisherFactory;
        _receiver = receiver;
    }

    public bool TryGetListener(
        string logicalName, [NotNullWhen(true)] out IMessageListener? listener)
    {
        listener = null;

        if (_listeners.TryGetValue(logicalName, out var lazy))
        {
            listener = lazy.Value.Result.Listener;
            return true;
        }

        return false;
    }

    public bool TryGetPublisher(
        string logicalName, [NotNullWhen(true)] out IMessagePublisher? publisher)
    {
        publisher = null;

        if (_publishers.TryGetValue(logicalName, out var lazy))
        {
            publisher = lazy.Value.Result;
            return true;
        }

        return false;
    }

    public async Task<IMessageListener> RegisterListenerForQueueAsync(
        string logicalName, string queue, CancellationToken cancellationToken = default)
    {
        var lazy = new Lazy<Task<(IMessageListener Listener, CancellationTokenSource Cts)>>(async () =>
        {
            var listener = _listenerFactory.CreateListenerForQueue(logicalName, queue);
            await listener.StartAsync(cancellationToken);

            var cts = new CancellationTokenSource();
            _ = ExecuteListenerPumpAsync(logicalName, listener, cts.Token);

            return (listener, cts);
        });

        if (!_listeners.TryAdd(logicalName, lazy))
        {
            throw new InvalidOperationException("Listener already registered");
        }

        var result = await lazy.Value;
        return result.Listener;
    }

    public async Task<IMessageListener> RegisterListenerForTopicAsync(
        string logicalName, string topic, string subscription, CancellationToken cancellationToken = default)
    {
        var lazy = new Lazy<Task<(IMessageListener Listener, CancellationTokenSource Cts)>>(async () =>
        {
            var listener = _listenerFactory.CreateListenerForTopic(logicalName, topic, subscription);
            await listener.StartAsync(cancellationToken);

            var cts = new CancellationTokenSource();
            _ = ExecuteListenerPumpAsync(logicalName, listener, cts.Token);

            return (listener, cts);
        });

        if (!_listeners.TryAdd(logicalName, lazy))
        {
            throw new InvalidOperationException("Listener already registered");
        }

        var result = await lazy.Value;
        return result.Listener;
    }

    public async Task<IMessagePublisher> RegisterPublisherForQueueAsync(
        string logicalName, string queue, CancellationToken cancellationToken = default)
    {
        var lazy = new Lazy<Task<IMessagePublisher>>(async () =>
        {
            var publisher = _publisherFactory.CreatePublisherForQueue(logicalName, queue);
            await publisher.StartAsync(cancellationToken);

            return publisher;
        });

        if (!_publishers.TryAdd(logicalName, lazy))
        {
            throw new InvalidOperationException("Publisher already registered");
        }

        var result = await lazy.Value;
        return result;
    }

    public async Task<IMessagePublisher> RegisterPublisherForTopicAsync(
        string logicalName, string topic, CancellationToken cancellationToken = default)
    {
        var lazy = new Lazy<Task<IMessagePublisher>>(async () =>
        {
            var publisher = _publisherFactory.CreatePublisherForTopic(logicalName, topic);
            await publisher.StartAsync(cancellationToken);

            return publisher;
        });

        if (!_publishers.TryAdd(logicalName, lazy))
        {
            throw new InvalidOperationException("Publisher already registered");
        }

        var result = await lazy.Value;
        return result;
    }

    public Task StartAsync(
        CancellationToken cancellationToken = default)
    {
        var parallelism = 8;

        _pump = new(
            async message =>
            {
                try
                {
                    await _receiver.ReceiveAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "The message receiver encountered an error while processing the message. The message " +
                        "will be abandoned, and the message will be returned to the broker.");

                    try
                    {
                        if (!message.Actions.Invoked)
                            await message.Actions.AbandonAsync();
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogError(ex2, "Failed to abandon message.");
                    }
                }
            },
            new()
            {
                MaxDegreeOfParallelism = parallelism,
                BoundedCapacity = parallelism,
            });

        _isRunning = true;

        return Task.CompletedTask;
    }

    public async Task StopAsync(
        CancellationToken cancellationToken = default)
    {
        _isRunning = false;

        _pump?.Complete();
        _pump = null;

        foreach (var logicalName in _publishers.Keys)
        {
            await UnregisterPublisherAsync(logicalName, cancellationToken);
        }

        foreach (var logicalName in _listeners.Keys)
        {
            await UnregisterListenerAsync(logicalName, cancellationToken);
        }
    }

    public async Task UnregisterListenerAsync(string logicalName, CancellationToken cancellationToken = default)
    {
        if (_listeners.Remove(logicalName, out var lazy))
        {
            try
            {
                var result = await lazy.Value;
                await result.Listener.StopAsync(cancellationToken);
            }
            catch { }
        }
    }

    public async Task UnregisterPublisherAsync(string logicalName, CancellationToken cancellationToken = default)
    {
        if (_publishers.Remove(logicalName, out var lazy))
        {
            try
            {
                var result = await lazy.Value;
                await result.StopAsync(cancellationToken);
            }
            catch { }
        }
    }

    private async Task ExecuteListenerPumpAsync(
        string logicalName,
        IMessageListener listener,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var message in listener.Channel.ReadAllAsync(cancellationToken))
                {
                    await _pump!.SendAsync(message, cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encountered an error during channel pump loop for {LogicalName}.", logicalName);
                await Task.Delay(2000, cancellationToken);
            }
        }
    }
}
