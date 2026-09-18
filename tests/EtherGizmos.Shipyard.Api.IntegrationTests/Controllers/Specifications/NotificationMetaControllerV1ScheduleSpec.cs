using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class NotificationMetaControllerV1ScheduleSpec : IODataResourceSpec<NotificationScheduleDTO, string>
{
    public static NotificationMetaControllerV1ScheduleSpec Instance { get; }

    static NotificationMetaControllerV1ScheduleSpec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/notificationSchedules";

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

    public Func<NotificationScheduleDTO, string> Identity => carrier => carrier.Id;

    public Func<string, string> Path => id => $"('{id}')";

    public IRecordSource<NotificationScheduleDTO, string> Records => new NotificationMetaControllerV1ScheduleSource(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(NotificationScheduleDTO entity) => throw new NotImplementedException();

    private class NotificationMetaControllerV1ScheduleSource : IRecordSource<NotificationScheduleDTO, string>
    {
        private readonly IODataResourceSpec<NotificationScheduleDTO, string> _specification;

        public NotificationMetaControllerV1ScheduleSource(
            IODataResourceSpec<NotificationScheduleDTO, string> specification)
        {
            _specification = specification;
        }

        public Task<(NotificationScheduleDTO Entity, string Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            return Task.FromResult((new NotificationScheduleDTO(), "immediate"));
        }
    }
}
