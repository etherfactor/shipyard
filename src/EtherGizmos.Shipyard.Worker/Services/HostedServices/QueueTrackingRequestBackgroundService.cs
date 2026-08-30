using EtherGizmos.Shipyard.Api;
using EtherGizmos.Shipyard.Extensions;
using EtherGizmos.Shipyard.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace EtherGizmos.Shipyard.Services.HostedServices;

public class QueueTrackingRequestBackgroundService : PeriodicBackgroundService
{
    private const string CRON_EXPRESSION = "*/60 * * * * *";

    public QueueTrackingRequestBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<QueueTrackingRequestBackgroundService> logger)
        : base(CRON_EXPRESSION, serviceProvider, logger)
    {
    }

    protected override async Task ExecuteIterationAsync(
        IServiceProvider provider,
        CancellationToken stoppingToken)
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        using var client = httpClientFactory.CreateClient("API");

        var now = DateTimeOffset.UtcNow;

        using var response = await client.GetAsync(
            $"/api/v1/packages" +
            $"?$filter=nextPollAt lt {now:yyyy-MM-ddTHH:mm:ss.fffffffZ}" +
            $"&$expand=carrier",
            cancellationToken: stoppingToken);

        var set = (await response.Content.ReadFromJsonAsync<ODataResultSet<PackageDTO>>(
            JsonSerializerOptions.App,
            cancellationToken: stoppingToken))!;

        var ready = set.Value;

        var parallelOptions = new ParallelOptions() { CancellationToken = stoppingToken };
        await Parallel.ForEachAsync(ready, parallelOptions, async (package, ct) =>
        {
            using var createResponse = await client.PostAsync(
                $"/api/v1/packages({package.Id})/schedulePoll",
                null,
                cancellationToken: ct);

            createResponse.EnsureSuccessStatusCode();
        });
    }
}
