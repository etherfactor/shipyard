using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Specifications;

public class GroupsControllerV1Spec : IODataResourceSpec<GroupDTO, int>
{
    public static GroupsControllerV1Spec Instance { get; }

    static GroupsControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/groups";

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
        };

    public Func<GroupDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<GroupDTO, int> Records => new GroupsControllerV1Source(this);

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

    private class GroupsControllerV1Source : IRecordSource<GroupDTO, int>
    {
        private readonly IODataResourceSpec<GroupDTO, int> _specification;

        public GroupsControllerV1Source(
            IODataResourceSpec<GroupDTO, int> specification)
        {
            _specification = specification;
        }

        public async Task<(GroupDTO Entity, int Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var body = _specification.Create();
            var client = context.GetClientWithCapabilities((createdByUserId ?? Setup.OwnerUserId).ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<GroupDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }
}
