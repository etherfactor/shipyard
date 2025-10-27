using Asp.Versioning;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Api.Controllers;

[Authorize]
public class CarriersController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/carriers";

    public CarriersController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<CarrierDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [ProducesResponseType(200, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<CarrierDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [Consumes(typeof(CarrierDTO), "application/json"), SwaggerRequestExample(typeof(CarrierDTO), typeof(CarrierDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] CarrierDTO create,
        ODataQueryOptions<CarrierDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [Consumes(typeof(CarrierDTO), "application/json"), SwaggerRequestExample(typeof(CarrierDTO), typeof(CarrierDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<CarrierDTO> patch,
        ODataQueryOptions<CarrierDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpDelete(BaseRoute + "({id})")]
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    private IKeylessRequestBuilder<Carrier, CarrierDTO> ForSet()
        => ForSet<Carrier, CarrierDTO>();

    private IKeyedRequestBuilder<Carrier, CarrierDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<Carrier, CarrierDTO, int>.Create(id, e => e.Id, e => e.Id));
}
