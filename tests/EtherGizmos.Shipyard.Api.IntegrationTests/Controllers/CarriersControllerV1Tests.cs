using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class CarriersControllerV1Tests : ODataControllerTestsBase<CarrierDTO, int>
{
    protected override IODataResourceSpec<CarrierDTO, int> Specification { get; } = new CarriersControllerV1Spec();

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var spec = new CarriersControllerV1Spec();
            var aspects = new IAspect<CarrierDTO, int>[] {
                new SearchSelectOptionAspect<CarrierDTO,int>(),
                new GetSelectOptionAspect<CarrierDTO,int>(),
                new CreateSelectOptionAspect<CarrierDTO,int>(),
                new PatchSelectOptionAspect<CarrierDTO,int>(),
                // …add more
            };

            foreach (var a in aspects)
                foreach (var c in a.Build(spec)) // your Build ignores ctx when creating cases
                    yield return c;
        }
    }


    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c) => await c.TestAsync(new FixtureContext(Client));

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
            var response = await context.Client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<CarrierDTO>();
            return (entity!, entity!.Id);
        }
    }

    private class CarriersControllerV1Spec : IODataResourceSpec<CarrierDTO, int>
    {
        public string BaseRoute => "api/v1/carriers";

        public IReadOnlySet<ODataCapability> Capabilities =>
            new HashSet<ODataCapability>()
            {
                ODataCapability.Search,
                ODataCapability.Get,
                ODataCapability.Create,
                ODataCapability.Update,
                ODataCapability.Delete,
                ODataCapability.QueryCount,
                ODataCapability.QueryExpand,
                ODataCapability.QueryFilter,
                ODataCapability.QueryOrderBy,
                ODataCapability.QuerySelect,
                ODataCapability.QuerySkip,
                ODataCapability.QueryTop,
            };

        public Func<CarrierDTO, int> Identity => carrier => carrier.Id;

        public Func<CarrierDTO, string> Path => carrier => $"({carrier.Id})";

        public IRecordSource<CarrierDTO, int> Records => throw new NotImplementedException();

        public HttpContent Create() =>
            JsonContent.Create(new
            {
                name = "Test Carrier",
            });

        public HttpContent Update(CarrierDTO entity) =>
            JsonContent.Create(new
            {
                name = "New Name",
            });
    }
}
