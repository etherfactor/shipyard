using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class SendStep : ScrapingStep
{
    [Required]
    public string Selector { get; set; } = null!;

    [Required]
    public string Value { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await client.SendAsync(Selector, Value, cancellationToken);
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
