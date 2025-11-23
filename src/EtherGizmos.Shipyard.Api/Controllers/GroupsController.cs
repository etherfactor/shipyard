using Asp.Versioning;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Api.Controllers;

[Authorize]
public class GroupsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/groups";

    public GroupsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Group, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(GroupDTO)), SwaggerResponseExample(200, typeof(GroupDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<GroupDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Group, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(GroupDTO)), SwaggerResponseExample(200, typeof(GroupDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<GroupDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Group, PermissionId.Write)]
    [Consumes(typeof(GroupDTO), "application/json"), SwaggerRequestExample(typeof(GroupDTO), typeof(GroupDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(GroupDTO)), SwaggerResponseExample(200, typeof(GroupDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] GroupDTO create,
        ODataQueryOptions<GroupDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Group, PermissionId.Write)]
    [Consumes(typeof(GroupDTO), "application/json"), SwaggerRequestExample(typeof(GroupDTO), typeof(GroupDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(GroupDTO)), SwaggerResponseExample(200, typeof(GroupDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<GroupDTO> patch,
        ODataQueryOptions<GroupDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Group, PermissionId.Delete)]
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<Group, GroupDTO> ForSet()
        => ForSet<Group, GroupDTO>();

    private IKeyedRequestBuilder<Group, GroupDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<Group, GroupDTO, int>.Create(id, e => e.Id, e => e.Id));
}
