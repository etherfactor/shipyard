using Asp.Versioning;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Api.Controllers;

[Authorize]
public class CarrierExecutionsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/carrierExecutions";

    public CarrierExecutionsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(CarrierExecutionDTO)), SwaggerResponseExample(200, typeof(CarrierExecutionDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<CarrierExecutionDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [ProducesResponseType(200, Type = typeof(CarrierExecutionDTO)), SwaggerResponseExample(200, typeof(CarrierExecutionDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<CarrierExecutionDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    private IKeylessRequestBuilder<CarrierExecution, CarrierExecutionDTO> ForSet()
        => ForSet<CarrierExecution, CarrierExecutionDTO>();

    private IKeyedRequestBuilder<CarrierExecution, CarrierExecutionDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<CarrierExecution, CarrierExecutionDTO, int>.Create(id, e => e.Id, e => e.Id));
}
