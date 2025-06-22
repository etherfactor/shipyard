namespace EtherGizmos.Messaging.Abstractions;

public interface IMessagePublisherFactory
{
    IMessagePublisher CreatePublisherForQueue(string logicalName, string queue);

    IMessagePublisher CreatePublisherForTopic(string logicalName, string topic);
}
