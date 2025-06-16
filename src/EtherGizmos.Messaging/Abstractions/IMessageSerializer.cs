namespace EtherGizmos.Messaging.Abstractions;

public interface IMessageSerializer
{
    string Serialize<TMessage>(TMessage messate)
        where TMessage : class, new();

    TMessage Deserialize<TMessage>(string message)
        where TMessage : class, new();
}
