using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class SendStep : ScrapingStep
{
    [Required]
    public string Selector { get; set; } = null!;

    [Required]
    public string Value { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var regex = new Regex(@"(?<!{){(?<key>[^{}]+)}(?!})");
        var newValue = regex.Replace(Value, match =>
        {
            var key = match.Groups["key"].Value;
            return variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        try
        {
            await client.SendAsync(Selector, newValue, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to send {Content} to element matching {Selector}", newValue, Selector);
        }
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
