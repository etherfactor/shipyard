using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EtherGizmos.Shipyard.Services.Notifications;

internal class NotificationUnsubscribeService : INotificationUnsubscribeService
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public NotificationUnsubscribeService(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<string> GetUnsubscribeKeyAsync(
        long subscriptionId,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var unsubKeyRepo = uow.Repository<NotificationUnsubscribeKey>();

        var record = await unsubKeyRepo.Data.SingleOrDefaultAsync(e =>
            e.SubscriptionId == subscriptionId,
            cancellationToken: cancellationToken);

        if (record is null)
        {
            record = new()
            {
                SubscriptionId = subscriptionId,
            };
            unsubKeyRepo.Add(record);

            await uow.SaveChangesAsync(cancellationToken);
        }

        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(record.Value));
    }

    public async Task<bool> UnsubscribeAsync(
        long subscriptionId,
        string key,
        CancellationToken cancellationToken = default)
    {
        var currentKey = await GetUnsubscribeKeyAsync(subscriptionId, cancellationToken);
        if (currentKey != key)
        {
            return false;
        }

        using var uow = _uowFactory.Create(new() { AmbientMode = UnitOfWorkAmbientMode.JoinOrCreateAmbient });

        var subscriptionRepo = uow.Repository<NotificationSubscription>();
        var subscription = await subscriptionRepo.Data.SingleAsync(e =>
            e.Id == subscriptionId,
            cancellationToken: cancellationToken);

        //Delete this unsubscribe key, so if the subscription gets turned back on, it gets a new one
        var unsubKeyRepo = uow.Repository<NotificationUnsubscribeKey>();
        await unsubKeyRepo.Data
            .Where(e => e.SubscriptionId == subscriptionId)
            .ExecuteDeleteAsync(cancellationToken);

        subscription.IsEnabled = false;

        await uow.SaveChangesAsync(cancellationToken);

        return true;
    }
}
