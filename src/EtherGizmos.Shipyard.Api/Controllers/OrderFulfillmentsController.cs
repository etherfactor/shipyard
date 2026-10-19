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

public class OrderFulfillmentsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/orderFulfillments";

    public OrderFulfillmentsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<OrderFulfillmentDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<OrderFulfillmentDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderFulfillmentDTO), "application/json"), SwaggerRequestExample(typeof(OrderFulfillmentDTO), typeof(OrderFulfillmentDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] OrderFulfillmentDTO create,
        ODataQueryOptions<OrderFulfillmentDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderFulfillmentDTO), "application/json"), SwaggerRequestExample(typeof(OrderFulfillmentDTO), typeof(OrderFulfillmentDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(OrderFulfillmentDTO)), SwaggerResponseExample(200, typeof(OrderFulfillmentDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<OrderFulfillmentDTO> patch,
        ODataQueryOptions<OrderFulfillmentDTO> queryOptions,
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

    private IKeylessRequestBuilder<OrderFulfillment, OrderFulfillmentDTO> ForSet()
        => ForSet<OrderFulfillment, OrderFulfillmentDTO>();

    private IKeyedRequestBuilder<OrderFulfillment, OrderFulfillmentDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<OrderFulfillment, OrderFulfillmentDTO, int>.Create(id, e => e.Id, e => e.Id));
}
