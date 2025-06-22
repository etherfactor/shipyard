namespace EtherGizmos.Messaging.Abstractions;

public interface IMessageMiddleware
{
    Task InvokeAsync(ReceivedMessage message, Func<Task> next);
}
