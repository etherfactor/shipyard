using Asp.Versioning;
using AutoMapper;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Api.Services.Security;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Messages;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Swashbuckle.AspNetCore.Filters;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Shipyard.Api.Controllers;

[Authorize]
public class PackagesController : AutoODataController
{
    private const string BaseRoute = "api/v{version:apiVersion}/packages";

    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IMapper _mapper;
    private readonly IMessageSender _sender;

    public PackagesController(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory,
        IMapper mapper,
        IMessageSender sender)
        : base(serviceProvider)
    {
        _uowFactory = uowFactory;
        _mapper = mapper;
        _sender = sender;
    }

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute)]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<PackageDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<PackageDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [Consumes(typeof(PackageDTO), "application/json"), SwaggerRequestExample(typeof(PackageDTO), typeof(PackageDTOExamplePost))]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExamplePost))]
    public Task<IActionResult> Create(
        [FromBody] PackageDTO create,
        ODataQueryOptions<PackageDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForSet()
            .CreateAsync(create, queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPatch(BaseRoute + "({id})")]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [Consumes(typeof(PackageDTO), "application/json"), SwaggerRequestExample(typeof(PackageDTO), typeof(PackageDTOExamplePatch))]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExampleGet))]
    public Task<IActionResult> Patch(
        int id,
        [FromBody] Delta<PackageDTO> patch,
        ODataQueryOptions<PackageDTO> queryOptions,
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

    [ApiVersion(1.0)]
    [HttpGet("api/v{version:apiVersion}/findUpdatedPackages")]
    [HasCapability(SecurableType.Package, PermissionId.Read)]
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExampleGet))]
    public async Task<IActionResult> FindUpdatedPackages(
        ODataQueryOptions<PackageDTO> queryOptions,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create(useRequestScope: true);
        var packageRepo = uow.Repository<Package>();

        var dbData = await packageRepo.Data
            .Include(e => e.TrackingUpdates)
            .OrderByDescending(package => package.TrackingUpdates.OrderByDescending(update => update.OccurredAt).Last().OccurredAt)
            .Take(queryOptions.Top.Value)
            .ToListAsync(cancellationToken: cancellationToken);

        var data = await _mapper.MapExplicitly(dbData.AsQueryable())
            .To<PackageDTO>()
            .ApplyQueryOptions(queryOptions)
            .ExecuteAsync(cancellationToken);

        return Ok(data);
    }

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute + "({id})" + "/schedulePoll")]
    [HasCapability(SecurableType.Package, PermissionId.Write)]
    [ProducesResponseType(202)]
    public async Task<IActionResult> SchedulePoll(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create(useRequestScope: true);
        var packageRepo = uow.Repository<Package>();

        var package = await packageRepo.Data
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);

        if (package is null)
        {
            new Error.Reference.EntityNotFoundReferenceError<PackageDTO>()
                .AddDetail((e => e.Id, id))
                .Return();
        }

        var executionRepo = uow.Repository<CarrierExecution>();

        var execution = new CarrierExecution()
        {
            CarrierId = package.CarrierId,
            ExecutionStatus = ExecutionStatusType.Queued,
            StepCount = (short)package.Carrier.Steps.Count,
        };

        executionRepo.Create(execution);

        await uow.SaveChangesAsync(cancellationToken);

        await _sender.SendAsync("tracking-poll-request", new TrackingRequest()
        {
            ExecutionId = execution.Id,
            PackageId = package.Id,
            CarrierId = package.CarrierId,
            TrackingNumber = package.TrackingNumber,
        }, cancellationToken: cancellationToken);

        package.LastPollAt = DateTimeOffset.UtcNow;
        package.NextPollAt = package.LastPollAt
            + TimeSpan.FromHours(6) * (double)package.LastStatusType.PollingFactor;

        await uow.SaveChangesAsync(cancellationToken);

        return Accepted();
    }

    private IKeylessRequestBuilder<Package, PackageDTO> ForSet()
        => ForSet<Package, PackageDTO>()
            .OnCreating(async (db, dto) =>
            {
                using var uow = _uowFactory.AsUnfiltered().Create();
                var userRepo = uow.Repository<User>();

                Guid.TryParse(User.GetClaim(Claims.Subject), out var userId);
                var groupId = await userRepo.Data
                    .Where(e => e.Id == userId)
                    .Select(e => e.GroupId)
                    .SingleAsync();

                db.GroupId = groupId;
            });

    private IKeyedRequestBuilder<Package, PackageDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<Package, PackageDTO, int>.Create(id, e => e.Id, e => e.Id));
}
