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

public class OrdersController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/orders";

    public OrdersController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(OrderDTO)), SwaggerResponseExample(200, typeof(OrderDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<OrderDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(OrderDTO)), SwaggerResponseExample(200, typeof(OrderDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<OrderDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderDTO), "application/json"), SwaggerRequestExample(typeof(OrderDTO), typeof(OrderDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(OrderDTO)), SwaggerResponseExample(200, typeof(OrderDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] OrderDTO create,
        ODataQueryOptions<OrderDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Order, PermissionId.Write)]
    [Consumes(typeof(OrderDTO), "application/json"), SwaggerRequestExample(typeof(OrderDTO), typeof(OrderDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(OrderDTO)), SwaggerResponseExample(200, typeof(OrderDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<OrderDTO> patch,
        ODataQueryOptions<OrderDTO> queryOptions,
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

    private IKeylessRequestBuilder<Order, OrderDTO> ForSet()
        => ForSet<Order, OrderDTO>();

    private IKeyedRequestBuilder<Order, OrderDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<Order, OrderDTO, int>.Create(id, e => e.Id, e => e.Id));
}
