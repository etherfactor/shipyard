using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class UsersControllerV1Spec : IODataResourceSpec<UserDTO, Guid>
{
    public static UsersControllerV1Spec Instance { get; }

    static UsersControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/users";

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
        };

    public Func<UserDTO, Guid> Identity => user => user.Id;

    public Func<Guid, string> Path => id => $"({id})";

    public IRecordSource<UserDTO, Guid> Records => new UsersControllerV1Source(this);

    public HttpContent Create() =>
        JsonContent.Create(new
        {
            username = Guid.NewGuid().ToString(),
            password = "Testing123!",
            groupId = 1,
        });

    public HttpContent Update(UserDTO entity) =>
        JsonContent.Create(new
        {
            fullName = "Full Name",
        });

    private class UsersControllerV1Source : IRecordSource<UserDTO, Guid>
    {
        private readonly IODataResourceSpec<UserDTO, Guid> _specification;

        public UsersControllerV1Source(
            IODataResourceSpec<UserDTO, Guid> specification)
        {
            _specification = specification;
        }

        public async Task<(UserDTO Entity, Guid Id)> AcquireAsync(
            FixtureContext context,
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            var body = _specification.Create();
            var client = context.GetClientWithCapabilities((createdByUserId ?? Setup.OwnerUserId).ToString());
            var response = await client.PostAsync(_specification.BaseRoute, body);

            var entity = await response.Content.ReadFromJsonAsync<UserDTO>(JsonOptions.Default);
            return (entity!, entity!.Id);
        }
    }
}
