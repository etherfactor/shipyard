namespace EtherGizmos.Common.Messaging.Abstractions;

public interface IMessageConsumer<TMessage>
    where TMessage : class, new()
{
    Task ConsumeAsync(IMessageContext<TMessage> context);
}
