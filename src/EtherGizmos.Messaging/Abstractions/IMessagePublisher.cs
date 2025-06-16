using System.Threading.Channels;

namespace EtherGizmos.Messaging.Abstractions;

public interface IMessagePublisher
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);

    ChannelWriter<SentMessage> Channel { get; }
}
