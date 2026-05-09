using Asp.Versioning;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Exceptions;
using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Api.Controllers;

[ApiController]
public class ImportExportController : ControllerBase
{
    private const string BaseRoute = "api/v{version:apiVersion}";

    private readonly ICapabilityAuthorizer _authorizer;
    private readonly IExportDocumentImporterRegistry _importerRegistry;
    private readonly IExportDocumentMigrator _migrator;
    private readonly IUnitOfWorkFactory _uowFactory;

    public ImportExportController(
        ICapabilityAuthorizer authorizer,
        IExportDocumentImporterRegistry importerRegistry,
        IExportDocumentMigrator migrator,
        IUnitOfWorkFactory uowFactory)
    {
        _authorizer = authorizer;
        _importerRegistry = importerRegistry;
        _migrator = migrator;
        _uowFactory = uowFactory;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "/carriers({id})/export")]
    [HasCapability(SecurableType.Carrier, PermissionId.Read)]
    [Produces("application/yaml", "application/json")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Export(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.AsUnfiltered().Create();
        var carrierRepo = uow.Repository<Carrier>();

        var carrier = await carrierRepo.Data
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);

        if (carrier is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<CarrierDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        var carrierExport = new CarrierExport(carrier);
        var node = JsonSerializer.SerializeToNode(carrierExport, JsonSerializerOptions.Export)!;

        var export = new ExportDocument(
            "carrier",
            1,
            JsonSerializer.Deserialize<IDictionary<string, object?>>(JsonNode.Parse($"{{\"exportedAt\":\"{DateTimeOffset.UtcNow}\"}}")!.AsObject(), JsonSerializerOptions.Export),
            JsonSerializer.Deserialize<IDictionary<string, object?>>(node, JsonSerializerOptions.Export)!);

        return Ok(export);
    }

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute + "/import")]
    [Consumes("application/yaml", "application/json")]
    [ProducesResponseType(201, Type = typeof(ImporterResultDTO)), SwaggerResponseExample(201, typeof(ImporterResultDTOExampleGet))]
    [ProducesResponseType(200, Type = typeof(ImporterResultDTO)), SwaggerResponseExample(200, typeof(ImporterResultDTOExampleGet))]
    public async Task<IActionResult> Import(
        ExportDocument data,
        CancellationToken cancellationToken = default)
    {
        var resultDto = new ImporterResultDTO()
        {
            Kind = data.Kind,
            SchemaVersion = data.SchemaVersion,
            Id = null,
            Identifier = null,
            Status = ImporterResultStatusType.Error,
            ErrorMessage = $"Unknown schema type: {data.Kind}.",
        };

        try
        {
            data = _migrator.MigrateDataToCurrent(data);

            var importer = _importerRegistry.GetImporter(data.Kind);

            if (importer is not null)
            {
                _authorizer.EnsureAuthorized(importer.SecurableType, PermissionId.Write);

                var result = await importer.ImportAsync(data, cancellationToken);

                resultDto = new ImporterResultDTO()
                {
                    Kind = result.Kind,
                    SchemaVersion = result.SchemaVersion,
                    Id = result.Id,
                    Identifier = result.Identifier,
                    Status = result.Status,
                    ErrorMessage = result.ErrorMessage,
                };
            }
        }
        catch (UnsupportedExportSchemaException ex)
        {
            resultDto.ErrorMessage = ex.Message;
        }

        return resultDto.Status switch
        {
            ImporterResultStatusType.Created => StatusCode(StatusCodes.Status201Created, resultDto),
            ImporterResultStatusType.Updated => Ok(resultDto),
            ImporterResultStatusType.Error => BadRequest(resultDto),
            _ => BadRequest(resultDto),
        };
    }

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute + "/import/verify")]
    [Consumes("application/yaml", "application/json")]
    [ProducesResponseType(201, Type = typeof(ImporterResultDTO)), SwaggerResponseExample(201, typeof(ImporterResultDTOExampleGet))]
    [ProducesResponseType(200, Type = typeof(ImporterResultDTO)), SwaggerResponseExample(200, typeof(ImporterResultDTOExampleGet))]
    public async Task<IActionResult> VerifyImport(
        ExportDocument data,
        CancellationToken cancellationToken = default)
    {
        var resultDto = new ImporterResultDTO()
        {
            Kind = data.Kind,
            SchemaVersion = data.SchemaVersion,
            Id = null,
            Identifier = null,
            Status = ImporterResultStatusType.Error,
            ErrorMessage = $"Unknown schema type: {data.Kind}.",
        };

        try
        {
            data = _migrator.MigrateDataToCurrent(data);

            var importer = _importerRegistry.GetImporter(data.Kind);

            if (importer is not null)
            {
                _authorizer.EnsureAuthorized(importer.SecurableType, PermissionId.Write);

                var result = await importer.VerifyAsync(data, cancellationToken);

                resultDto = new ImporterResultDTO()
                {
                    Kind = result.Kind,
                    SchemaVersion = result.SchemaVersion,
                    Id = result.Id,
                    Identifier = result.Identifier,
                    Status = result.Status,
                    ErrorMessage = result.ErrorMessage,
                };
            }
        }
        catch (UnsupportedExportSchemaException ex)
        {
            resultDto.ErrorMessage = ex.Message;
        }

        return resultDto.Status switch
        {
            ImporterResultStatusType.Created => StatusCode(StatusCodes.Status201Created, resultDto),
            ImporterResultStatusType.Updated => Ok(resultDto),
            ImporterResultStatusType.Error => BadRequest(resultDto),
            _ => BadRequest(resultDto),
        };
    }
}
