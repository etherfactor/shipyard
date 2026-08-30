using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class PackagesControllerV1Spec : IODataResourceSpec<PackageDTO, int>
{
    public static PackagesControllerV1Spec Instance { get; }

    static PackagesControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/packages";

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

    public Func<PackageDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<PackageDTO, int> Records => new PackagesControllerV1Source(this);

    public HttpContent Create() =>
        JsonContent.Create(new
        {
            carrierId = 1,
            trackingNumber = "test" + Guid.NewGuid().ToString("N"),
        });

    public HttpContent Update(PackageDTO entity) =>
        JsonContent.Create(new
        {
            trackingNumber = "test" + Guid.NewGuid().ToString("N"),
        });

    private class PackagesControllerV1Source : IRecordSource<PackageDTO, int>
    {
        private readonly IODataResourceSpec<PackageDTO, int> _specification;

        public PackagesControllerV1Source(
            IODataResourceSpec<PackageDTO, int> specification)
        {
            _specification = specification;
        }

        public async Task<(PackageDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var body = _specification.Create();
            var client = context.GetClientWithCapabilities((createdByUserId ?? Setup.OwnerUserId).ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<PackageDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }
}
