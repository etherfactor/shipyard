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

public class OrderItemsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/orderItems";

    public OrderItemsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(OrderItemDTO)), SwaggerResponseExample(200, typeof(OrderItemDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<OrderItemDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(OrderItemDTO)), SwaggerResponseExample(200, typeof(OrderItemDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<OrderItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderItemDTO), "application/json"), SwaggerRequestExample(typeof(OrderItemDTO), typeof(OrderItemDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(OrderItemDTO)), SwaggerResponseExample(200, typeof(OrderItemDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] OrderItemDTO create,
        ODataQueryOptions<OrderItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderItemDTO), "application/json"), SwaggerRequestExample(typeof(OrderItemDTO), typeof(OrderItemDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(OrderItemDTO)), SwaggerResponseExample(200, typeof(OrderItemDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<OrderItemDTO> patch,
        ODataQueryOptions<OrderItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Delete)]
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<OrderItem, OrderItemDTO> ForSet()
        => ForSet<OrderItem, OrderItemDTO>();

    private IKeyedRequestBuilder<OrderItem, OrderItemDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<OrderItem, OrderItemDTO, int>.Create(id, e => e.Id, e => e.Id));
}
