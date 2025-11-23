using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class GroupsControllerV1Tests : ODataControllerTestsBase<GroupDTO, int>
{
    private static readonly IODataResourceSpec<GroupDTO, int> _specification = new GroupsControllerV1Spec();
    private static readonly FixtureContext _context = new(() => Setup.Client, new JwtTokenMinter());

    protected override IODataResourceSpec<GroupDTO, int> Specification => _specification;

    public static IEnumerable<AspectCase> All
    {
        get
        {
            var aspects = new IAspect<GroupDTO, int>[] {
                new SearchAuthenticationAspect<GroupDTO, int>(),
                new SearchConformanceAspect<GroupDTO, int>(),
                new SearchSelectOptionAspect<GroupDTO, int>(),

                new GetAuthenticationAspect<GroupDTO, int>(),
                new GetConformanceAspect<GroupDTO, int>(),
                new GetRecordNotFoundAspect<GroupDTO, int>(),
                new GetSelectOptionAspect<GroupDTO, int>(),

                new CreateAuthenticationAspect<GroupDTO, int>(),
                new CreateConformanceAspect<GroupDTO, int>(),
                new CreateSelectOptionAspect<GroupDTO, int>(),

                new PatchAuthenticationAspect<GroupDTO, int>(),
                new PatchConformanceAspect<GroupDTO, int>(),
                new PatchRecordNotFoundAspect<GroupDTO, int>(),
                new PatchSelectOptionAspect<GroupDTO, int>(),

                new DeleteAuthenticationAspect<GroupDTO, int>(),
                new DeleteConformanceAspect<GroupDTO, int>(),
                new DeleteRecordNotFoundAspect<GroupDTO, int>(),
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

    private class PackagesControllerV1Source : IRecordSource<GroupDTO, int>
    {
        private readonly IODataResourceSpec<GroupDTO, int> _specification;

        public PackagesControllerV1Source(
            IODataResourceSpec<GroupDTO, int> specification)
        {
            _specification = specification;
        }

        public async Task<(GroupDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose)
        {
            var body = _specification.Create();
            var client = context.GetClientWithCapabilities(Setup.OwnerUserId.ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<GroupDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }

    private class GroupsControllerV1Spec : IODataResourceSpec<GroupDTO, int>
    {
        public string BaseRoute => "api/v1/groups";

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

        public Func<GroupDTO, int> Identity => carrier => carrier.Id;

        public Func<int, string> Path => id => $"({id})";

        public IRecordSource<GroupDTO, int> Records => new PackagesControllerV1Source(_specification);

        public HttpContent Create() =>
            JsonContent.Create(new
            {
                name = "Test",
            });

        public HttpContent Update(GroupDTO entity) =>
            JsonContent.Create(new
            {
                name = "New Name",
            });
    }
}
