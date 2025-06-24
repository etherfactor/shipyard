using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class WaitForStep : ScrapingStep
{
    [Required]
    public string Selector { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, CancellationToken cancellationToken = default)
    {
        await client.WaitForElementAsync(Selector, cancellationToken: cancellationToken);
    }
}
