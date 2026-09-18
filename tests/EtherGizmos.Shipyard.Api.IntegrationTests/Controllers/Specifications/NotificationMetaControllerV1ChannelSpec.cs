using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class NotificationMetaControllerV1ChannelSpec : IODataResourceSpec<NotificationChannelDTO, string>
{
    public static NotificationMetaControllerV1ChannelSpec Instance { get; }

    static NotificationMetaControllerV1ChannelSpec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/notificationChannels";

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

    public Func<NotificationChannelDTO, string> Identity => carrier => carrier.Id;

    public Func<string, string> Path => id => $"('{id}')";

    public IRecordSource<NotificationChannelDTO, string> Records => new NotificationMetaControllerV1ChannelSource(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(NotificationChannelDTO entity) => throw new NotImplementedException();

    private class NotificationMetaControllerV1ChannelSource : IRecordSource<NotificationChannelDTO, string>
    {
        private readonly IODataResourceSpec<NotificationChannelDTO, string> _specification;

        public NotificationMetaControllerV1ChannelSource(
            IODataResourceSpec<NotificationChannelDTO, string> specification)
        {
            _specification = specification;
        }

        public Task<(NotificationChannelDTO Entity, string Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            return Task.FromResult((new NotificationChannelDTO(), "webhook"));
        }
    }
}
