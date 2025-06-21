using EtherGizmos.Messaging.Abstractions;
using RabbitMQ.Client;

namespace EtherGizmos.Messaging.Services;

internal class RabbitMQMessageActions : IMessageActions
{
    private readonly IChannel _channel;
    private readonly ulong _messageId;

    public bool Invoked { get; private set; }

    public RabbitMQMessageActions(
        IChannel channel,
        ulong messageId)
    {
        _channel = channel;
        _messageId = messageId;
    }

    public async Task AbandonAsync(CancellationToken cancellationToken = default)
    {
        if (Invoked)
            throw new InvalidOperationException("Already performed an action on this message.");

        Invoked = true;

        await _channel.BasicNackAsync(_messageId, false, false, cancellationToken);
    }

    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        if (Invoked)
            throw new InvalidOperationException("Already performed an action on this message.");

        Invoked = true;

        await _channel.BasicAckAsync(_messageId, false, cancellationToken);
    }

    public async Task DeadLetterAsync(CancellationToken cancellationToken = default)
    {
        if (Invoked)
            throw new InvalidOperationException("Already performed an action on this message.");

        Invoked = true;

        await _channel.BasicNackAsync(_messageId, false, true, cancellationToken);
    }
}
