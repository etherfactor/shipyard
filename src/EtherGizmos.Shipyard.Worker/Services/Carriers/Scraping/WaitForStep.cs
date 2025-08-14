using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class WaitForStep : ScrapingStep
{
    [Required]
    public string Selector { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await client.WaitForElementAsync(Selector, timeout: TimeSpan.FromSeconds(15), cancellationToken: cancellationToken);
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
