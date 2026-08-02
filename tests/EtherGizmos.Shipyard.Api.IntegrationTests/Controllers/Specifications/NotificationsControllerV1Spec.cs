using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Models;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

internal class NotificationsControllerV1Spec : IODataResourceSpec<NotificationDTO, long>
{
    public static NotificationsControllerV1Spec Instance { get; }

    static NotificationsControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/notifications";

    public IReadOnlySet<ResourceFunctionality> Capabilities =>
        new HashSet<ResourceFunctionality>()
        {
            //Actions
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,

            //Qeury options
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,
        };

    public Func<NotificationDTO, long> Identity => Notification => Notification.Id;

    public Func<long, string> Path => id => $"({id})";

    public IRecordSource<NotificationDTO, long> Records => new NotificationsControllerV1Source(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(NotificationDTO entity) => throw new NotImplementedException();

    private class NotificationsControllerV1Source : IRecordSource<NotificationDTO, long>
    {
        private readonly IODataResourceSpec<NotificationDTO, long> _specification;

        public NotificationsControllerV1Source(
            IODataResourceSpec<NotificationDTO, long> specification)
        {
            _specification = specification;
        }

        public async Task<(NotificationDTO Entity, long Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var eventId = "package.delivered";
            var channelId = "webhook";
            var scheduleId = "immediate";

            var uowFactory = Setup.Services.GetRequiredService<IUnitOfWorkFactory>();
            using var uow = uowFactory.Create();

            var (_, subscriptionId) = await NotificationSubscriptionsControllerV1Spec.Instance.Records.AcquireAsync(context, purpose, createdByUserId);

            var notificationRepo = uow.Repository<Notification>();

            var notification = new Notification()
            {
                SubscriptionId = subscriptionId,
                EventId = eventId,
                ChannelId = channelId,
                ScheduleId = scheduleId,
                Status = NotificationStatusType.Sent,
                PayloadType = "test",
                Payload = "{}",
            };

            notificationRepo.Add(notification);

            await uow.SaveChangesAsync();

            return (new() { }, notification.Id);
        }
    }
}
