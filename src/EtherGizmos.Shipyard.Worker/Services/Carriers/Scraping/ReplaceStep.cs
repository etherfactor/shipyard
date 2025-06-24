using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ReplaceStep : ScrapingStep, ISettableStep
{
    [Required]
    public string Var { get; set; } = null!;

    [Required]
    public string From { get; set; } = null!;

    [Required]
    public string To { get; set; } = null!;

    public bool Trim { get; set; }

    public override Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var builder = new StringBuilder();

        var regex = new Regex(@"(?<!{){(?<key>[^{}]+)}(?!})");
        var newFrom = regex.Replace(From, match =>
        {
            var key = match.Groups["key"].Value;
            return variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        var newTo = regex.Replace(To, match =>
        {
            var key = match.Groups["key"].Value;
            return variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        var newValue = variables.TryGetValue(Var, out var value) ? value?.ToString() ?? "" : "";
        newValue = newValue.Replace(newFrom, newTo);

        if (Trim)
        {
            newValue = newValue.Trim();
        }

        variables[Var] = newValue;

        return Task.CompletedTask;
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
