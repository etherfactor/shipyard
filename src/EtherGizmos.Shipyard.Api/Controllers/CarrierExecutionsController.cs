using Asp.Versioning;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Services.Security;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

[Authorize]
public class CarrierExecutionsController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/carrierExecutions";

    public CarrierExecutionsController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    { }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Carrier, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(CarrierExecutionDTO)), SwaggerResponseExample(200, typeof(CarrierExecutionDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<CarrierExecutionDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Carrier, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(CarrierExecutionDTO)), SwaggerResponseExample(200, typeof(CarrierExecutionDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<CarrierExecutionDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    //[ApiVersion(1.0)]
    //[HttpPost(BaseRoute)]
    //[HasCapability(SecurableType.Carrier, PermissionId.Write)]
    //[Consumes(typeof(CarrierExecutionDTO), "application/json"), SwaggerRequestExample(typeof(CarrierExecutionDTO), typeof(CarrierExecutionDTOExamplePost))]
    //[ProducesResponseType(200, Type = typeof(CarrierExecutionDTO)), SwaggerResponseExample(200, typeof(CarrierExecutionDTOExamplePost))]
    //public Task<IActionResult> Create(
    //    [FromBody] CarrierExecutionDTO create,
    //    ODataQueryOptions<CarrierExecutionDTO> queryOptions,
    //    CancellationToken cancellationToken = default)
    //    => ForSet()
    //        .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Carrier, PermissionId.Write)]
    [Consumes(typeof(CarrierExecutionDTO), "application/json"), SwaggerRequestExample(typeof(CarrierExecutionDTO), typeof(CarrierExecutionDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(CarrierExecutionDTO)), SwaggerResponseExample(200, typeof(CarrierExecutionDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<CarrierExecutionDTO> patch,
        ODataQueryOptions<CarrierExecutionDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .PatchAsync(patch, queryOptions, cancellationToken);

    private IKeylessRequestBuilder<CarrierExecution, CarrierExecutionDTO> ForSet()
        => ForSet<CarrierExecution, CarrierExecutionDTO>();
    //        .OnCreating((db, dto) =>
    //        {
    //            db.StartedAt ??= db.CompletedAt;
    //            db.StepCount = (short)db.Carrier.Steps.Count;
    //            return Task.CompletedTask;
    //        });

    private IKeyedRequestBuilder<CarrierExecution, CarrierExecutionDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<CarrierExecution, CarrierExecutionDTO, int>.Create(id, e => e.Id, e => e.Id))
            .OnUpdating((db, dto) =>
            {
                db.StartedAt ??= db.CompletedAt;
                db.StepCount = (short)db.Carrier.Steps.Count;
                return Task.CompletedTask;
            });
}
