using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;

namespace EtherGizmos.Shipyard.Controllers.Specifications;

public class RolesControllerV1Spec : IODataResourceSpec<RoleDTO, int>
{
    public static RolesControllerV1Spec Instance { get; }

    static RolesControllerV1Spec()
    {
        Instance = new();
    }

    public string BaseRoute => "api/v1/roles";

    public IReadOnlySet<ResourceFunctionality> Capabilities =>
        new HashSet<ResourceFunctionality>()
        {
            //Actions
            ResourceFunctionality.Search,
            ResourceFunctionality.Get,

            //Query options
            ResourceFunctionality.QueryCount,
            ResourceFunctionality.QueryExpand,
            ResourceFunctionality.QueryFilter,
            ResourceFunctionality.QueryOrderBy,
            ResourceFunctionality.QuerySelect,
            ResourceFunctionality.QuerySkip,
            ResourceFunctionality.QueryTop,

            //Miscellaneous
            ResourceFunctionality.CapabilityRequired,
        };

    public Func<RoleDTO, int> Identity => carrier => carrier.Id;

    public Func<int, string> Path => id => $"({id})";

    public IRecordSource<RoleDTO, int> Records => new RolesControllerV1Source(this);

    public HttpContent Create() => throw new NotImplementedException();

    public HttpContent Update(RoleDTO entity) => throw new NotImplementedException();

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
            AcquirePurpose purpose,
            Guid? createdByUserId = null)
        {
            return Task.FromResult((new RoleDTO(), 1));
        }
    }
}
