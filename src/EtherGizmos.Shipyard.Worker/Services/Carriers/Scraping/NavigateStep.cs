using EtherGizmos.Shipyard.Services.WebDrivers;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Services.Carriers.Scraping;

internal class NavigateStep : ScrapingStep
{
    public override string StepName => $"Navigate → {Url}";

    [Required]
    public string Url { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var builder = new StringBuilder();

        var regex = new Regex(@"(?<!{){(?<key>[^{}]+)}(?!})");
        var newUrl = regex.Replace(Url, match =>
        {
            var key = match.Groups["key"].Value;
            return variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        await client.NavigateAsync(newUrl, cancellationToken: cancellationToken);
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
