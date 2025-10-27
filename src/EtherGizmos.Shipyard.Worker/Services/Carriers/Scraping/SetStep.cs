using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class SetStep : ScrapingStep, ISettableStep
{
    [Required]
    public string Var { get; set; } = null!;

    [Required]
    public string Value { get; set; } = null!;

    public bool Trim { get; set; } = false;

    public override Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var regex = new Regex(@"(?<!{){(?<key>[^{}]+)}(?!})");
        var newValue = regex.Replace(Value, match =>
        {
            var key = match.Groups["key"].Value;
            return variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        if (Trim)
        {
            newValue = newValue.Trim();
        }

        Logger.LogInformation("Setting variable {Variable} to value {Value}", Var, newValue);

        variables[Var] = newValue;

        return Task.CompletedTask;
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
