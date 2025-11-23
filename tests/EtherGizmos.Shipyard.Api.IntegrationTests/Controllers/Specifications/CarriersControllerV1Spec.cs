using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;

public class CarriersControllerV1Spec : IODataResourceSpec<CarrierDTO, int>
{
    public static CarriersControllerV1Spec Instance { get; }

    static CarriersControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/carriers";

    public IReadOnlySet<ResourceFunctionality> Capabilities =>
        new HashSet<ResourceFunctionality>()
        {
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,
            ResourceFunctionality.Create,
            ResourceFunctionality.Update,
            ResourceFunctionality.Delete,
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,
        };

    public Func<CarrierDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<CarrierDTO, int> Records => new CarriersControllerV1Source(this);

    public HttpContent Create() =>
        JsonContent.Create(new
        {
            name = "Test Carrier",
            slug = "test" + Guid.NewGuid().ToString("N").Substring(0, 16),
            rules = new List<object>(),
            steps = new List<object>(),
        });

    public HttpContent Update(CarrierDTO entity) =>
        JsonContent.Create(new
        {
            name = "New Name",
        });

    private class CarriersControllerV1Source : IRecordSource<CarrierDTO, int>
    {
        private readonly IODataResourceSpec<CarrierDTO, int> _specification;

        public CarriersControllerV1Source(
            IODataResourceSpec<CarrierDTO, int> specification)
        {
            _specification = specification;
        }

        public async Task<(CarrierDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose)
        {
            var body = _specification.Create();
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<CarrierDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }
}
