using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class RolesControllerV1Tests : ODataControllerTestsBase<RoleDTO, int>
{
    private static readonly IODataResourceSpec<RoleDTO, int> _specification = new RolesControllerV1Spec();
    private static readonly FixtureContext _context = new(() => Setup.Client, new JwtTokenMinter());

    protected override IODataResourceSpec<RoleDTO, int> Specification => _specification;

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var aspects = new IAspect<RoleDTO, int>[] {
                new SearchAuthenticationAspect<RoleDTO, int>(),
                new SearchConformanceAspect<RoleDTO, int>(),
                new SearchSelectOptionAspect<RoleDTO, int>(),

                new GetAuthenticationAspect<RoleDTO, int>(),
                new GetConformanceAspect<RoleDTO, int>(),
                new GetRecordNotFoundAspect<RoleDTO, int>(),
                new GetSelectOptionAspect<RoleDTO, int>(),
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

    private class RolesControllerV1Source : IRecordSource<RoleDTO, int>
    {
        private readonly IODataResourceSpec<RoleDTO, int> _specification;

        public RolesControllerV1Source(
            IODataResourceSpec<RoleDTO, int> specification)
        {
            _specification = specification;
        }

        public Task<(RoleDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose)
        {
            return Task.FromResult((new RoleDTO(), 1));
        }
    }

    private class RolesControllerV1Spec : IODataResourceSpec<RoleDTO, int>
    {
        public string BaseRoute => "api/v1/roles";

        public IReadOnlySet<ODataCapability> Capabilities =>
            new HashSet<ODataCapability>()
            {
                ODataCapability.Search,
                ODataCapability.Get,
                ODataCapability.QueryCount,
                ODataCapability.QueryExpand,
                ODataCapability.QueryFilter,
                ODataCapability.QueryOrderBy,
                ODataCapability.QuerySelect,
                ODataCapability.QuerySkip,
                ODataCapability.QueryTop,
            };

        public Func<RoleDTO, int> Identity => carrier => carrier.Id;

        public Func<int, string> Path => id => $"({id})";

        public IRecordSource<RoleDTO, int> Records => new RolesControllerV1Source(_specification);

        public HttpContent Create() => throw new NotImplementedException();

        public HttpContent Update(RoleDTO entity) => throw new NotImplementedException();
    }
}
