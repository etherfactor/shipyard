using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class PackagesControllerV1Tests : ODataControllerTestsBase<PackageDTO, int>
{
    private static readonly IODataResourceSpec<PackageDTO, int> _specification = new PackagesControllerV1Spec();
    private static readonly FixtureContext _context = new(() => Setup.Client, new JwtTokenMinter());

    protected override IODataResourceSpec<PackageDTO, int> Specification => _specification;

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var aspects = new IAspect<PackageDTO, int>[] {
                new SearchAuthenticationAspect<PackageDTO, int>(),
                new SearchConformanceAspect<PackageDTO, int>(),
                new SearchSelectOptionAspect<PackageDTO, int>(),

                new GetAuthenticationAspect<PackageDTO, int>(),
                new GetConformanceAspect<PackageDTO, int>(),
                new GetRecordNotFoundAspect<PackageDTO, int>(),
                new GetSelectOptionAspect<PackageDTO, int>(),

                new CreateAuthenticationAspect<PackageDTO, int>(),
                new CreateConformanceAspect<PackageDTO, int>(),
                new CreateSelectOptionAspect<PackageDTO, int>(),

                new PatchAuthenticationAspect<PackageDTO, int>(),
                new PatchConformanceAspect<PackageDTO, int>(),
                new PatchRecordNotFoundAspect<PackageDTO, int>(),
                new PatchSelectOptionAspect<PackageDTO, int>(),

                new DeleteAuthenticationAspect<PackageDTO, int>(),
                new DeleteConformanceAspect<PackageDTO, int>(),
                new DeleteRecordNotFoundAspect<PackageDTO, int>(),
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

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await _specification.Records.AcquireAsync(_context, AcquirePurpose.ForRead);
    }

    [TestCaseSource(nameof(All))]
    public async Task Aspect(AspectCase c) => await c.TestAsync(_context);

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
            AcquirePurpose purpose)
        {
            var body = _specification.Create();
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<PackageDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }

    private class PackagesControllerV1Spec : IODataResourceSpec<PackageDTO, int>
    {
        public string BaseRoute => "api/v1/packages";

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

        public Func<PackageDTO, int> Identity => carrier => carrier.Id;

        public Func<int, string> Path => id => $"({id})";

        public IRecordSource<PackageDTO, int> Records => new PackagesControllerV1Source(_specification);

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
    }
}
