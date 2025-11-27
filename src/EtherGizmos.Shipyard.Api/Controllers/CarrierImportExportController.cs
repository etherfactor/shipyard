using Asp.Versioning;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Models;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Api.Controllers;

[ApiController]
public class CarrierImportExportController : ControllerBase
{
    private const string BaseRoute = "api/v{version:apiVersion}/carriers";

    private readonly IUnitOfWorkFactory _uowFactory;

    public CarrierImportExportController(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})/export")]
    //[HasCapability(SecurableType.Carrier, PermissionId.Read)]
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
}
