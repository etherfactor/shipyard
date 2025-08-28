using Asp.Versioning;
using AutoMapper;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Api.Errors;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Messages;
using EtherGizmos.Shipyard.Models.Api.Errors;
using EtherGizmos.Shipyard.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace EtherGizmos.Shipyard.Api.Controllers;

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
    [ProducesResponseSet]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExampleGet))]
    public Task<IActionResult> Search(
    ODataQueryOptions<PackageDTO> queryOptions,
    CancellationToken cancellationToken = default)
    => ForSet()
        .SearchAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet(BaseRoute + "({id})")]
    [ProducesResponseType(200, Type = typeof(PackageDTO)), SwaggerResponseExample(200, typeof(PackageDTOExampleGet))]
    public Task<IActionResult> Get(
        int id,
        ODataQueryOptions<PackageDTO> queryOptions,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .GetAsync(queryOptions, cancellationToken);

    [ApiVersion(1.0)]
    [HttpPost(BaseRoute)]
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
    [ProducesResponseType(204)]
    public Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
        => ForItem(id)
            .DeleteAsync(cancellationToken);

    [ApiVersion(1.0)]
    [HttpGet("api/v{version:apiVersion}/findUpdatedPackages")]
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

        await _sender.SendAsync("tracking-poll-request", new TrackingRequest()
        {
            PackageId = package.Id,
            CarrierSlug = package.Carrier.Slug,
            TrackingNumber = package.TrackingNumber,
        }, cancellationToken: cancellationToken);

        package.LastPollAt = DateTimeOffset.UtcNow;
        package.NextPollAt = package.LastPollAt
            + TimeSpan.FromHours(6) * (double)package.LastStatusType.PollingFactor;

        await uow.SaveChangesAsync(cancellationToken);

        return Accepted();
    }

    private IKeylessRequestBuilder<Package, PackageDTO> ForSet()
        => ForSet<Package, PackageDTO>();

    private IKeyedRequestBuilder<Package, PackageDTO> ForItem(
        int id)
        => ForItem(
            KeyMapping<Package, PackageDTO, int>.Create(id, e => e.Id, e => e.Id));
}
