using Asp.Versioning;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Models.Api.Errors;
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

    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IArtifactReader _artifactReader;

    public CarrierExecutionsController(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory,
        IArtifactReader artifactReader)
        : base(serviceProvider)
    {
        _uowFactory = uowFactory;
        _artifactReader = artifactReader;
    }

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

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})" + "/readArtifact")]
    [HasCapability(SecurableType.Carrier, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(Stream))]
    public async Task<IActionResult> ReadArtifact(
        int id,
        string uri,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var record = await LoadRecordAsync(
            uow,
            [KeyMapping<CarrierExecution, CarrierExecutionDTO, int>.Create(id, e => e.Id, e => e.Id)],
            cancellationToken: cancellationToken);

        var artifact = record.Artifacts.FirstOrDefault(e => e.ArtifactUri.ToString().Equals(uri, StringComparison.OrdinalIgnoreCase));
        if (artifact is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<CarrierExecutionDTO>()
                .AddDetail((e => e.Artifacts[0].ArtifactUri, uri))
                .Return();
        }

        var meta = await _artifactReader.ReadAsync(artifact.ArtifactUri, cancellationToken);

        return File(meta.Stream, meta.ContentType, meta.FileName);
    }

    private IKeylessRequestBuilder<CarrierExecution, CarrierExecutionDTO> ForSet()
        => ForSet<CarrierExecution, CarrierExecutionDTO>();

    private IKeyedRequestBuilder<CarrierExecution, CarrierExecutionDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<CarrierExecution, CarrierExecutionDTO, int>.Create(id, e => e.Id, e => e.Id));
}
