using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class TrackingUpdatesControllerV1Tests : ODataControllerTestsBase<TrackingUpdateDTO, int>
{
    private static readonly IODataResourceSpec<TrackingUpdateDTO, int> _specification = new TrackingUpdatesControllerV1Spec();
    private static readonly FixtureContext _context = new(() => Setup.Client, new JwtTokenMinter());

    protected override IODataResourceSpec<TrackingUpdateDTO, int> Specification => _specification;

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var aspects = new IAspect<TrackingUpdateDTO, int>[] {
                new SearchAuthenticationAspect<TrackingUpdateDTO, int>(),

                new GetAuthenticationAspect<TrackingUpdateDTO, int>(),
                new GetRecordNotFoundAspect<TrackingUpdateDTO, int>(),
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

    private class TrackingUpdatesControllerV1Source : IRecordSource<TrackingUpdateDTO, int>
    {
        private readonly IODataResourceSpec<TrackingUpdateDTO, int> _specification;

        public TrackingUpdatesControllerV1Source(
            IODataResourceSpec<TrackingUpdateDTO, int> specification)
        {
            _specification = specification;
        }

        public Task<(TrackingUpdateDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose)
        {
            return Task.FromResult((new TrackingUpdateDTO(), 1));
        }
    }

    private class TrackingUpdatesControllerV1Spec : IODataResourceSpec<TrackingUpdateDTO, int>
    {
        public string BaseRoute => "api/v1/trackingUpdates";

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

        public Func<TrackingUpdateDTO, int> Identity => carrier => carrier.Id;

        public Func<int, string> Path => id => $"({id})";

        public IRecordSource<TrackingUpdateDTO, int> Records => new TrackingUpdatesControllerV1Source(_specification);

        public HttpContent Create() => throw new NotImplementedException();

        public HttpContent Update(TrackingUpdateDTO entity) => throw new NotImplementedException();
    }
}
