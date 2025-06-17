using EtherGizmos.Messaging.Abstractions;

namespace EtherGizmos.Messaging.Services;

internal class RabbitMQTransport : IMessageListenerFactory, IMessagePublisherFactory
{
    public IMessageListener CreateListenerForQueue(string logicalName, string queue)
    {
        throw new NotImplementedException();
    }

    public IMessageListener CreateListenerForTopic(string logicalName, string topic, string subscription)
    {
        throw new NotImplementedException();
    }

    public IMessagePublisher CreatePublisherForQueue(string logicalName, string queue)
    {
        throw new NotImplementedException();
    }

    public IMessagePublisher CreatePublisherForTopic(string logicalName, string topic)
    {
        throw new NotImplementedException();
    }
}
