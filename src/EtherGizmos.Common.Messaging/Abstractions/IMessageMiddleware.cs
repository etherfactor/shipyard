namespace EtherGizmos.Common.Messaging.Abstractions;

public interface IMessageMiddleware
{
    Task InvokeAsync(ReceivedMessage message, Func<Task> next);
}
