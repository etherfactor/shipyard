using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class NavigateStep : ScrapingStep
{
    [Required]
    public string Url { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, CancellationToken cancellationToken = default)
    {
        await client.NavigateAsync(Url, cancellationToken: cancellationToken);
    }
}
