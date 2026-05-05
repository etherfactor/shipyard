using Asp.Versioning;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Services.Security;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

[Authorize]
public class RolesController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/roles";

    public RolesController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Role, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(RoleDTO)), SwaggerResponseExample(200, typeof(RoleDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<RoleDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Role, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(RoleDTO)), SwaggerResponseExample(200, typeof(RoleDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<RoleDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    //[ApiVersion(1.0)]
    //[HttpPost(BaseRoute)]
    //[HasCapability(SecurableType.Role, PermissionId.Write)]
    //[Consumes(typeof(RoleDTO), "application/json"), SwaggerRequestExample(typeof(RoleDTO), typeof(RoleDTOExamplePost))]
    //[ProducesResponseType(200, Type = typeof(RoleDTO)), SwaggerResponseExample(200, typeof(RoleDTOExamplePost))]
    //public Task<IActionResult> Create(
    //    [FromBody] RoleDTO create,
    //    ODataQueryOptions<RoleDTO> queryOptions,
    //    CancellationToken cancellationToken = default)
    //    => ForSet()
    //        .CreateAsync(create, queryOptions, cancellationToken);

    //[ApiVersion(1.0)]
    //[HttpPatch(BaseRoute + "({id})")]
    //[HasCapability(SecurableType.Role, PermissionId.Write)]
    //[Consumes(typeof(RoleDTO), "application/json"), SwaggerRequestExample(typeof(RoleDTO), typeof(RoleDTOExamplePatch))]
    //[ProducesResponseType(200, Type = typeof(RoleDTO)), SwaggerResponseExample(200, typeof(RoleDTOExampleGet))]
    //public Task<IActionResult> Patch(
    //    int id,
    //    [FromBody] Delta<RoleDTO> patch,
    //    ODataQueryOptions<RoleDTO> queryOptions,
    //    CancellationToken cancellationToken = default)
    //    => ForItem(id)
    //        .PatchAsync(patch, queryOptions, cancellationToken);

    //[ApiVersion(1.0)]
    //[HttpDelete(BaseRoute + "({id})")]
    //[HasCapability(SecurableType.Role, PermissionId.Delete)]
    //[ProducesResponseType(204)]
    //public Task<IActionResult> Delete(
    //    int id,
    //    CancellationToken cancellationToken = default)
    //    => ForItem(id)
    //        .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<Role, RoleDTO> ForSet()
        => ForSet<Role, RoleDTO>();

    private IKeyedRequestBuilder<Role, RoleDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<Role, RoleDTO, int>.Create(id, e => e.Id, e => e.Id));
}
