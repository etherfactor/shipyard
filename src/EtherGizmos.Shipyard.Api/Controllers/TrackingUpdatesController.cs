using Asp.Versioning;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Services.Security;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Controllers;

[Authorize]
public class TrackingUpdatesController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/trackingUpdates";

    private readonly IUnitOfWorkFactory _uowFactory;

    public TrackingUpdatesController(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory)
        : base(serviceProvider)
    {
        _uowFactory = uowFactory;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(TrackingUpdateDTO)), SwaggerResponseExample(200, typeof(TrackingUpdateDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<TrackingUpdateDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(TrackingUpdateDTO)), SwaggerResponseExample(200, typeof(TrackingUpdateDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<TrackingUpdateDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [Consumes(typeof(TrackingUpdateDTO), "application/json"), SwaggerRequestExample(typeof(TrackingUpdateDTO), typeof(TrackingUpdateDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(TrackingUpdateDTO)), SwaggerResponseExample(200, typeof(TrackingUpdateDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] TrackingUpdateDTO create,
        ODataQueryOptions<TrackingUpdateDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [Consumes(typeof(TrackingUpdateDTO), "application/json"), SwaggerRequestExample(typeof(TrackingUpdateDTO), typeof(TrackingUpdateDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(TrackingUpdateDTO)), SwaggerResponseExample(200, typeof(TrackingUpdateDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<TrackingUpdateDTO> patch,
        ODataQueryOptions<TrackingUpdateDTO> queryOptions,
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

    private IKeylessRequestBuilder<TrackingUpdate, TrackingUpdateDTO> ForSet()
        => ForSet<TrackingUpdate, TrackingUpdateDTO>()
            .OnCreating(async (db, dto) =>
            {
                using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
                var packageRepo = uow.Repository<Package>();

                var packageExists = await packageRepo.Data.AnyAsync(e => e.Id == db.PackageId);
                if (!packageExists)
                {
                    new Error.Reference.EntityNotFoundReferenceError<TrackingUpdateDTO>()
                        .AddDetail((e => e.PackageId, db.PackageId))
                        .Return();
                }
            });

    private IKeyedRequestBuilder<TrackingUpdate, TrackingUpdateDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<TrackingUpdate, TrackingUpdateDTO, int>.Create(id, e => e.Id, e => e.Id))
            .OnUpdating(async (db, dto) =>
            {
                using var uow = _uowFactory.Create(new() { SccopeMode = UnitOfWorkScopeMode.RequestScope });
                var packageRepo = uow.Repository<Package>();

                var packageExists = await packageRepo.Data.AnyAsync(e => e.Id == db.PackageId);
                if (!packageExists)
                {
                    new Error.Reference.EntityNotFoundReferenceError<TrackingUpdateDTO>()
                        .AddDetail((e => e.PackageId, db.PackageId))
                        .Return();
                }
            });
}
