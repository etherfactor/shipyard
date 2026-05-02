using Asp.Versioning;
using AutoMapper;
using EtherGizmos.Common.Converters;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EtherGizmos.Shipyard.Api.Controllers;

[ApiController]
public class ImportExportController : ControllerBase
{
    private const string BaseRoute = "api/v{version:apiVersion}";

    private readonly IMapper _mapper;
    private readonly IUnitOfWorkFactory _uowFactory;

    public ImportExportController(
        IMapper mapper,
        IUnitOfWorkFactory uowFactory)
    {
        _mapper = mapper;
        _uowFactory = uowFactory;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "/carriers({id})/export")]
    //[HasCapability(SecurableType.Carrier, PermissionId.Read)]
    [Produces("application/yaml", "application/json")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ExportAsync(
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

        var carrierExport = new CarrierExportV1(carrier);
        var node = JsonSerializer.SerializeToNode(carrierExport, JsonSerializerOptions.Web)!;

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new ObjectToInferredTypesConverter());

        var export = new ExportDocument(
            "carrier",
            1,
            JsonSerializer.Deserialize<IDictionary<string, object?>>(JsonNode.Parse($"{{\"exportedAt\":\"{DateTimeOffset.UtcNow}\"}}")!.AsObject(), jsonOptions),
            JsonSerializer.Deserialize<IDictionary<string, object?>>(node, jsonOptions)!);
        return Ok(export);
    }

    [ApiVersion(1.0)]
    [HttpPut(BaseRoute + "/import")]
    //[HasCapability(SecurableType.Carrier, PermissionId.Write)]
    [Consumes("application/yaml", "application/json")]
    //[ProducesResponseType(200, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    //[ProducesResponseType(201, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    public async Task<IActionResult> ImportAsync(
        ExportDocument data,
        CancellationToken cancellationToken = default)
    {
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new ObjectToInferredTypesConverter());

        if (data.Kind == "carrier" && data.SchemaVersion == 1)
        {
            var node = JsonSerializer.SerializeToNode(data.Data, jsonOptions)!.ToJsonString();
            var carrierData = JsonSerializer.Deserialize<CarrierExportV1>(node, JsonSerializerOptions.Web)!;

            using var uow = _uowFactory.AsUnfiltered().Create();
            var carrierRepo = uow.Repository<Carrier>();

            var carrier = await carrierRepo.Data
                .SingleOrDefaultAsync(e => e.Slug == carrierData.Slug, cancellationToken: cancellationToken);

            var isNew = false;
            if (carrier is null)
            {
                isNew = true;

                carrier = new();
                carrierRepo.Create(carrier);
            }

            carrierData.Apply(carrier);

            await uow.SaveChangesAsync(cancellationToken);

            var finished = _mapper
                .MapExplicitly(carrier)
                .To<CarrierDTO>()
                .Execute();

            return isNew ? Created() : NoContent();
        }

        return BadRequest();
    }
}
