using Asp.Versioning;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Controllers;

[ApiController]
[Tags("CarrierExecutions")]
public class CarrierExecutionArtifactsController : ControllerBase
{
    private const string BaseRoute = "api/v{version:apiVersion}/carrierExecutions";

    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IArtifactReader _artifactReader;
    private readonly IArtifactWriter _artifactWriter;
    private readonly IUnitOfWorkAccessor _uowAccessor;

    public CarrierExecutionArtifactsController(
        IUnitOfWorkFactory uowFactory,
        IArtifactReader artifactReader,
        IArtifactWriter artifactWriter,
        IUnitOfWorkAccessor uowAccessor)
    {
        _uowFactory = uowFactory;
        _artifactReader = artifactReader;
        _artifactWriter = artifactWriter;
        _uowAccessor = uowAccessor;
    }

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
        var executionRepo = uow.Repository<CarrierExecution>();

        var record = await executionRepo.Data.SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);
        if (record is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<CarrierExecutionDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

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

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute + "({id})" + "/writeArtifact")]
    [HasCapability(SecurableType.Carrier, PermissionId.Write)]
    [ProducesResponseType(202)]
    public async Task<IActionResult> WriteArtifact(
        int id,
        [FromForm] ArtifactRequestDTO request,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var executionRepo = uow.Repository<CarrierExecution>();

        var record = await executionRepo.Data.SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);
        if (record is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<CarrierExecutionDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        using var content = request.File.OpenReadStream();
        var contentType = request.File.ContentType;
        var fileName = request.File.FileName;

        var format = ArtifactFormat.FromContentType(contentType);
        var descriptor = await _artifactWriter.WriteForRunAsync(
            id,
            format,
            fileName ?? $"{Guid.NewGuid()}.{format.Extension}",
            content,
            cancellationToken: cancellationToken);

        record.Artifacts.Add(new()
        {
            ArtifactUri = descriptor.Uri,
            ContentType = descriptor.ContentType,
            FileName = descriptor.FileName,
            Bytes = descriptor.Bytes,
        });

        var a = _uowAccessor;
        await uow.SaveChangesAsync(cancellationToken);

        return StatusCode(201, new ArtifactResponseDTO()
        {
            ArtifactUri = descriptor.Uri.Value,
            ContentType = descriptor.ContentType,
            FileName = descriptor.FileName,
            Bytes = descriptor.Bytes,
        });
    }
}
