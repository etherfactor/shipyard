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

public class OrderFulfillmentItemsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/orderFulfillmentItems";

    public OrderFulfillmentItemsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentItemDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentItemDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<OrderFulfillmentItemDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentItemDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentItemDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<OrderFulfillmentItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderFulfillmentItemDTO), "application/json"), SwaggerRequestExample(typeof(OrderFulfillmentItemDTO), typeof(OrderFulfillmentItemDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentItemDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentItemDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] OrderFulfillmentItemDTO create,
        ODataQueryOptions<OrderFulfillmentItemDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderFulfillmentItemDTO), "application/json"), SwaggerRequestExample(typeof(OrderFulfillmentItemDTO), typeof(OrderFulfillmentItemDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentItemDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentItemDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<OrderFulfillmentItemDTO> patch,
        ODataQueryOptions<OrderFulfillmentItemDTO> queryOptions,
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

    private IKeylessRequestBuilder<OrderFulfillmentItem, OrderFulfillmentItemDTO> ForSet()
        => ForSet<OrderFulfillmentItem, OrderFulfillmentItemDTO>();

    private IKeyedRequestBuilder<OrderFulfillmentItem, OrderFulfillmentItemDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<OrderFulfillmentItem, OrderFulfillmentItemDTO, int>.Create(id, e => e.Id, e => e.Id));
}
