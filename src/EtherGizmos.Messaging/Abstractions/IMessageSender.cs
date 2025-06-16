namespace EtherGizmos.Messaging.Abstractions;

public interface IMessageSender
{
    IServiceProvider Services { get; }

    Task SendAsync(SentMessage message, CancellationToken cancellationToken = default);
}

public static class IMessageSenderExtensions
{
    public static async Task SendAsync<TMessage>(
        this IMessageSender @this,
        string logicalName,
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class, new()
    {


        await @this.SendAsync(new SentMessage()
        {
            //TODO: Populate this
        }, cancellationToken: cancellationToken);
    }
}
