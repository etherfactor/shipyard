using Asp.Versioning;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Services.Security;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

public class PackageItemsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/packageItems";

    public PackageItemsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(PackageItemDTO)), SwaggerResponseExample(200, typeof(PackageItemDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<PackageItemDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(PackageItemDTO)), SwaggerResponseExample(200, typeof(PackageItemDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<PackageItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [Consumes(typeof(PackageItemDTO), "application/json"), SwaggerRequestExample(typeof(PackageItemDTO), typeof(PackageItemDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(PackageItemDTO)), SwaggerResponseExample(200, typeof(PackageItemDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] PackageItemDTO create,
        ODataQueryOptions<PackageItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [Consumes(typeof(PackageItemDTO), "application/json"), SwaggerRequestExample(typeof(PackageItemDTO), typeof(PackageItemDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(PackageItemDTO)), SwaggerResponseExample(200, typeof(PackageItemDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<PackageItemDTO> patch,
        ODataQueryOptions<PackageItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Delete)]
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<PackageItem, PackageItemDTO> ForSet()
        => ForSet<PackageItem, PackageItemDTO>();

    private IKeyedRequestBuilder<PackageItem, PackageItemDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<PackageItem, PackageItemDTO, int>.Create(id, e => e.Id, e => e.Id));
}
