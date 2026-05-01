using Asp.Versioning;
using AutoMapper;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace EtherGizmos.Shipyard.Api.Controllers;

[ApiController]
public class CarrierImportExportController : ControllerBase
{
    private const string BaseRoute = "api/v{version:apiVersion}/carriers";

    private readonly IMapper _mapper;
    private readonly IUnitOfWorkFactory _uowFactory;

    public CarrierImportExportController(
        IMapper mapper,
        IUnitOfWorkFactory uowFactory)
    {
        _mapper = mapper;
        _uowFactory = uowFactory;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})/export")]
    [HasCapability(SecurableType.Carrier, PermissionId.Read)]
    [Produces("application/yaml")]
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

        var export = new CarrierExportV1(carrier);
        return Ok(export);
    }

    [ApiVersion(1.0)]
    [HttpPut(BaseRoute + "import")]
    [HasCapability(SecurableType.Carrier, PermissionId.Write)]
    [Consumes("application/yaml")]
    //[ProducesResponseType(200, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    //[ProducesResponseType(201, Type = typeof(CarrierDTO)), SwaggerResponseExample(200, typeof(CarrierDTOExampleGet))]
    public async Task<IActionResult> ImportAsync(
        CarrierExportV1 data,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.AsUnfiltered().Create();
        var carrierRepo = uow.Repository<Carrier>();

        var carrier = await carrierRepo.Data
            .SingleOrDefaultAsync(e => e.Slug == data.Slug, cancellationToken: cancellationToken);

        var isNew = false;
        if (carrier is null)
        {
            isNew = true;

            carrier = new();
            carrierRepo.Create(carrier);
        }

        data.Apply(carrier);

        await uow.SaveChangesAsync(cancellationToken);

        var finished = _mapper
            .MapExplicitly(carrier)
            .To<CarrierDTO>()
            .Execute();

        return StatusCode(isNew ? (int)HttpStatusCode.Created : (int)HttpStatusCode.OK, finished);
    }
}
