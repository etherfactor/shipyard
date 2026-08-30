using EtherGizmos.Shipyard.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Services.Carriers.Scraping;

internal class ReplaceStep : ScrapingStep, ISettableStep
{
    public override string StepName => $"[DEPRECATED] Replace";

    [Required]
    public string Var { get; set; } = null!;

    [Required]
    public string From { get; set; } = null!;

    [Required]
    public string To { get; set; } = null!;

    public bool IsRegex { get; set; }

    public bool Trim { get; set; }

    public override Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var newValue = variables.TryGetValue(Var, out var value) ? value?.ToString() ?? "" : "";

        if (IsRegex)
        {
            Logger.LogInformation("Replacing {From} with {To} in value {Value}", From, To, newValue);
            newValue = new Regex(From, RegexOptions.None).Replace(newValue, To);
        }
        else
        {
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

            Logger.LogInformation("Replacing {From} with {To} in value {Value}", newFrom, newTo, newValue);
            newValue = newValue.Replace(newFrom, newTo);
        }

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
