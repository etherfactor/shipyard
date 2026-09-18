using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class NotificationMetaControllerV1EventSpec : IODataResourceSpec<NotificationEventDTO, string>
{
    public static NotificationMetaControllerV1EventSpec Instance { get; }

    static NotificationMetaControllerV1EventSpec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/notificationEvents";

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

    public Func<NotificationEventDTO, string> Identity => carrier => carrier.Id;

    public Func<string, string> Path => id => $"('{id}')";

    public IRecordSource<NotificationEventDTO, string> Records => new NotificationMetaControllerV1EventSource(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(NotificationEventDTO entity) => throw new NotImplementedException();

    private class NotificationMetaControllerV1EventSource : IRecordSource<NotificationEventDTO, string>
    {
        private readonly IODataResourceSpec<NotificationEventDTO, string> _specification;

        public NotificationMetaControllerV1EventSource(
            IODataResourceSpec<NotificationEventDTO, string> specification)
        {
            _specification = specification;
        }

        public Task<(NotificationEventDTO Entity, string Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            return Task.FromResult((new NotificationEventDTO(), "package.delivered"));
        }
    }
}
