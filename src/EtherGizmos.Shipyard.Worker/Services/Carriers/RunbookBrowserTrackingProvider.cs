using EtherGizmos.Shipyard.Database.Services;
using EtherGizmos.Shipyard.Models.Database;
using EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;
using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

internal class RunbookBrowserTrackingProvider : ITrackingProvider
{
    private static readonly JsonSerializerOptions _jsonOptions;

    private readonly IServiceProvider _serviceProvider;
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IBrowserClient _browserClient;
    private readonly string _slug;

    static RunbookBrowserTrackingProvider()
    {
        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        _jsonOptions.Converters.Add(new ScrapingStepConverter());
    }

    public RunbookBrowserTrackingProvider(
        IServiceProvider serviceProvider,
        string slug)
    {
        _serviceProvider = serviceProvider;
        _uowFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        _browserClient = serviceProvider.GetRequiredService<IBrowserClient>();
        _slug = slug;
    }

    public async Task<TrackingResult> TrackAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();
        var carrierRepo = uow.Repository<Carrier>();

        var runbookJson = "[]";

        var runbook = JsonSerializer.Deserialize<List<ScrapingStep>>(runbookJson, _jsonOptions);
    }
}
