using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ClickStep : ScrapingStep
{
    [Required]
    public string Selector { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.ClickElementAsync(Selector, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to click element matching {Selector}", Selector);
        }
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
