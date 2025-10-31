using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class CarrierExecutionsControllerTests : ODataControllerTestsBase<CarrierExecutionDTO, int>
{
    private static readonly IODataResourceSpec<CarrierExecutionDTO, int> _specification = new CarrierExecutionsControllerV1Spec();
    private static readonly FixtureContext _context = new(() => Setup.Client, new JwtTokenMinter());

    protected override IODataResourceSpec<CarrierExecutionDTO, int> Specification => _specification;

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var aspects = new IAspect<CarrierExecutionDTO, int>[] {
                new SearchAuthenticationAspect<CarrierExecutionDTO, int>(),

                new GetAuthenticationAspect<CarrierExecutionDTO, int>(),
                new GetRecordNotFoundAspect<CarrierExecutionDTO, int>(),
            };

            foreach (var aspect in aspects)
            {
                foreach (var test in aspect.Build(_specification))
                {
                    yield return test;
                }
            }
        }
    }

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c) => await c.TestAsync(_context);

    private class CarrierExecutionsControllerV1Source : IRecordSource<CarrierExecutionDTO, int>
    {
        private readonly IODataResourceSpec<CarrierExecutionDTO, int> _specification;

        public CarrierExecutionsControllerV1Source(
            IODataResourceSpec<CarrierExecutionDTO, int> specification)
        {
            _specification = specification;
        }

        public Task<(CarrierExecutionDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose)
        {
            return Task.FromResult((new CarrierExecutionDTO(), 1));
        }
    }

    private class CarrierExecutionsControllerV1Spec : IODataResourceSpec<CarrierExecutionDTO, int>
    {
        public string BaseRoute => "api/v1/carrierExecutions";

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

        public Func<CarrierExecutionDTO, int> Identity => carrier => carrier.Id;

        public Func<int, string> Path => id => $"({id})";

        public IRecordSource<CarrierExecutionDTO, int> Records => new CarrierExecutionsControllerV1Source(_specification);

        public HttpContent Create() => throw new NotImplementedException();

        public HttpContent Update(CarrierExecutionDTO entity) => throw new NotImplementedException();
    }
}
