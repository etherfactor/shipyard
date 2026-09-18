namespace EtherGizmos.Common.Abstractions;

public interface INotificationUnsubscribeService
{
    Task<string> GetUnsubscribeKeyAsync(
        long subscriptionId,
        CancellationToken cancellationToken = default);

    Task<bool> UnsubscribeAsync(
        long subscriptionId,
        string key,
        CancellationToken cancellationToken = default);
}
