namespace EtherGizmos.Common.Messaging.Abstractions;

public interface IMessageListenerFactory
{
    IMessageListener CreateListenerForQueue(string logicalName, string queue);

    IMessageListener CreateListenerForTopic(string logicalName, string topic, string subscription);
}
