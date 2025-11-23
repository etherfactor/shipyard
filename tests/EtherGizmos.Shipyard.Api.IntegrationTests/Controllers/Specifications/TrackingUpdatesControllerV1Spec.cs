using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Enums;
using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;

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
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,
            ResourceFunctionality.GroupFiltering,
        };

    public Func<TrackingUpdateDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<TrackingUpdateDTO, int> Records => new TrackingUpdatesControllerV1Source(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(TrackingUpdateDTO entity) => throw new NotImplementedException();

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
            AcquirePurpose purpose)
        {
            var (_, id) = await PackagesControllerV1Spec.Instance.Records.AcquireAsync(context, purpose);

            var uowFactory = Setup.Services.GetRequiredService<IUnitOfWorkFactory>();
            using var uow = uowFactory.Create();

            var updateRepo = uow.Repository<TrackingUpdate>();

            var update = new TrackingUpdate()
            {
                PackageId = id,
                StatusTypeId = StatusTypeId.Delivered,
            };

            updateRepo.Create(update);

            await uow.SaveChangesAsync();

            return (new()
            {
                StatusType = StatusTypeDTO.Delivered,
            }, update.Id);
        }
    }
}
