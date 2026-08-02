using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class TrackingUpdatesControllerV1Spec : IODataResourceSpec<TrackingUpdateDTO, int>
{
    public static TrackingUpdatesControllerV1Spec Instance { get; }

    static TrackingUpdatesControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/trackingUpdates";

    public IReadOnlySet<ResourceFunctionality> Capabilities =>
        new HashSet<ResourceFunctionality>()
        {
            //Actions
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,
            ResourceFunctionality.Create,
            ResourceFunctionality.Update,
            ResourceFunctionality.Delete,

            //Query options
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,

            //Miscellaneous
            ResourceFunctionality.GroupFiltering,
            ResourceFunctionality.CapabilityRequired,
        };

    public Func<TrackingUpdateDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<TrackingUpdateDTO, int> Records => new TrackingUpdatesControllerV1Source(this);

    public HttpContent Create() => Create(packageId: 1);

    private HttpContent Create(int packageId) =>
        JsonContent.Create(new
        {
            occurredAt = DateTimeOffset.UtcNow.ToString("O"),
            statusType = "OutForDelivery",
            location = "Somewhere",
            description = "Your package was delivered",
            packageId = packageId,
        });

    public HttpContent Update(TrackingUpdateDTO entity) =>
        JsonContent.Create(new
        {
            statusType = "Delivered",
        });

    private class TrackingUpdatesControllerV1Source : IRecordSource<TrackingUpdateDTO, int>
    {
        private readonly IODataResourceSpec<TrackingUpdateDTO, int> _specification;

        public TrackingUpdatesControllerV1Source(
            IODataResourceSpec<TrackingUpdateDTO, int> specification)
        {
            _specification = specification;
        }

        public async Task<(TrackingUpdateDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var (_, id) = await PackagesControllerV1Spec.Instance.Records.AcquireAsync(context, purpose, createdByUserId);

            var body = ((TrackingUpdatesControllerV1Spec)_specification).Create(packageId: id);
            var client = context.GetClientWithCapabilities((createdByUserId ?? Setup.OwnerUserId).ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<TrackingUpdateDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }
}
