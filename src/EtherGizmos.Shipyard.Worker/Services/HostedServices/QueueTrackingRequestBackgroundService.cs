using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Configuration;
using EtherGizmos.Shipyard.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Services.HostedServices;

public class QueueTrackingRequestBackgroundService : PeriodicBackgroundService
{
    private const string CRON_EXPRESSION = "*/60 * * * * *";

    private readonly IMessageSender _sender;

    public QueueTrackingRequestBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<QueueTrackingRequestBackgroundService> logger,
        IMessageSender sender)
        : base(CRON_EXPRESSION, serviceProvider, logger)
    {
        _sender = sender;
    }

    protected override async Task ExecuteIterationAsync(
        IServiceProvider provider,
        CancellationToken stoppingToken)
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        using var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(provider.GetRequiredService<IOptionsMonitor<ApiOptions>>().CurrentValue.BaseUrl);

        var now = DateTimeOffset.UtcNow;

        var response = await client.GetAsync($"/api/v1/packages?$filter=nextPollAt lt {now:yyyy-MM-ddTHH:mm:ss.fffffffZ}&$expand=carrier", stoppingToken);

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        var set = (await response.Content.ReadFromJsonAsync<ODataResultSet<PackageDTO>>(jsonOptions, cancellationToken: stoppingToken))!;
        var ready = set.Value;

        //var uowFactory = provider.GetRequiredService<IUnitOfWorkFactory>().AsUnfiltered();
        //using var uow = uowFactory.Create();

        //var packageRepo = uow.Repository<Package>();

        //var ready = packageRepo.Data
        //    .Where(e => e.NextPollAt < DateTimeOffset.UtcNow)
        //    .Include(e => e.Carrier)
        //    .Include(e => e.LastStatusType)
        //    .AsAsyncEnumerable();

        await Parallel.ForEachAsync(ready, async (package, ct) =>
        {
            //using var subUow = uowFactory.Create();
            //var executionRepo = subUow.Repository<CarrierExecution>();

            //var execution = new CarrierExecution()
            //{
            //    CarrierId = package.CarrierId,
            //    PackageId = package.Id,
            //    ExecutionStatus = ExecutionStatusType.Queued,
            //    StepCount = (short)package.Carrier.Steps.Count,
            //};

            //executionRepo.Add(execution);

            //await subUow.SaveChangesAsync(ct);

            //await _sender.SendAsync("tracking-poll-request", new TrackingRequest()
            //{
            //    ExecutionId = execution.Id,
            //    PackageId = package.Id,
            //    CarrierId = package.CarrierId,
            //    TrackingNumber = package.TrackingNumber,
            //}, cancellationToken: stoppingToken);

            //package.LastPollAt = DateTimeOffset.UtcNow;
            //package.NextPollAt = package.LastPollAt
            //    + TimeSpan.FromHours(6) * (double)package.LastStatusType.PollingFactor;
        });

        //await uow.SaveChangesAsync(stoppingToken);
    }
}
