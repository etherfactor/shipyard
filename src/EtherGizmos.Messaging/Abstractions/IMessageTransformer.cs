namespace EtherGizmos.Messaging.Abstractions;

public interface IMessageTransformer
{
    Task<SentMessage> WrapAsync(SentMessage envelope, CancellationToken cancellationToken = default);

    Task<SentMessage> UnwrapAsync(SentMessage envelope, CancellationToken cancellationToken = default);
}
