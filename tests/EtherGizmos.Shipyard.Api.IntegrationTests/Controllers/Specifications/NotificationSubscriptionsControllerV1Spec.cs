using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

internal class NotificationSubscriptionsControllerV1Spec : IODataResourceSpec<NotificationSubscriptionDTO, long>
{
    public static NotificationSubscriptionsControllerV1Spec Instance { get; }

    static NotificationSubscriptionsControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/notificationSubscriptions";

    public IReadOnlySet<ResourceFunctionality> Capabilities =>
        new HashSet<ResourceFunctionality>()
        {
            //Actions
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,
            ResourceFunctionality.Create,
            ResourceFunctionality.Update,
            ResourceFunctionality.Delete,

            //Qeury options
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,
        };

    public Func<NotificationSubscriptionDTO, long> Identity => NotificationSubscription => NotificationSubscription.Id;

    public Func<long, string> Path => id => $"({id})";

    public IRecordSource<NotificationSubscriptionDTO, long> Records => new NotificationSubscriptionsControllerV1Source(this);

    public HttpContent Create() =>
        JsonContent.Create(new
        {
            notificationEventId = "package.delivered",
            notificationEventConfig = new { },
            notificationChannelId = "webhook",
            notificationChannelConfig = new
            {
                endpoint = "http://localhost",
                method = "POST",
                headers = new { },
            },
            notificationScheduleId = "immediate",
            notificationScheduleConfig = new { },
        });

    public HttpContent Update(NotificationSubscriptionDTO entity) =>
        JsonContent.Create(new
        {
            isActive = false,
        });

    private class NotificationSubscriptionsControllerV1Source : IRecordSource<NotificationSubscriptionDTO, long>
    {
        private readonly IODataResourceSpec<NotificationSubscriptionDTO, long> _specification;

        public NotificationSubscriptionsControllerV1Source(
            IODataResourceSpec<NotificationSubscriptionDTO, long> specification)
        {
            _specification = specification;
        }

        public async Task<(NotificationSubscriptionDTO Entity, long Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var eventId = "package.delivered";
            var channelId = "webhook";
            var scheduleId = "immediate";

            var uowFactory = Setup.Services.GetRequiredService<IUnitOfWorkFactory>();
            using var uow = uowFactory.Create();

            var subscriptionRepo = uow.Repository<NotificationSubscription>();

            var subscription = new NotificationSubscription()
            {
                UserId = Guid.NewGuid().ToString(),
                EventId = eventId,
                EventConfig = new Dictionary<string, object?>() { },
                ChannelId = channelId,
                ChannelConfig = new Dictionary<string, object?>()
                {
                    ["endpoint"] = "http://localhost",
                    ["method"] = "POST",
                    ["headers"] = new Dictionary<string, object?>() { },
                },
                ScheduleId = scheduleId,
                ScheduleConfig = new Dictionary<string, object?>() { },
            };

            subscriptionRepo.Add(subscription);

            await uow.SaveChangesAsync();

            return (new() { }, subscription.Id);
        }
    }
}
