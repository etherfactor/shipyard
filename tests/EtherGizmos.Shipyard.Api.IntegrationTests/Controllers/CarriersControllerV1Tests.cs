using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class CarriersControllerV1Tests : ODataControllerTestsBase<CarrierDTO, int>
{
    private static readonly IODataResourceSpec<CarrierDTO, int> _specification = new CarriersControllerV1Spec();
    private static readonly FixtureContext _context = new(() => Setup.Client, new JwtTokenMinter());

    protected override IODataResourceSpec<CarrierDTO, int> Specification => _specification;

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var aspects = new IAspect<CarrierDTO, int>[] {
                new SearchSelectOptionAspect<CarrierDTO,int>(),
                new GetSelectOptionAspect<CarrierDTO,int>(),
                new CreateSelectOptionAspect<CarrierDTO,int>(),
                new PatchSelectOptionAspect<CarrierDTO,int>(),
                new AuthenticationAspect<CarrierDTO,int>(),
                // …add more
            };

            foreach (var a in aspects)
                foreach (var c in a.Build(_specification)) // your Build ignores ctx when creating cases
                    yield return c;
        }
    }

    [SetUp]
    public async Task SetUp()
    {
        await _specification.Records.AcquireAsync(_context, AcquirePurpose.ForRead);
    }

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c) => await c.TestAsync(_context);

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
            var client = context.GetClientAsRole("123", 1);
            var response = await client.PostAsync(_specification.BaseRoute, body);

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

        public IRecordSource<CarrierDTO, int> Records => new CarriersControllerV1Source(_specification);

        public HttpContent Create() =>
            JsonContent.Create(new
            {
                name = "Test Carrier",
                slug = "test" + new Random().Next(999999),
                rules = new List<object>(),
                steps = new List<object>(),
            });

        public HttpContent Update(CarrierDTO entity) =>
            JsonContent.Create(new
            {
                name = "New Name",
            });
    }
}
