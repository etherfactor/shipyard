using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using System.ComponentModel.DataAnnotations;
using System.Text;
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
        var builder = new StringBuilder();

        var regex = new Regex(@"(?<!{){(?<key>[^{}]+)}(?!})");
        var newValue = regex.Replace(Value, match =>
        {
            var key = match.Groups["key"].Value;
            return variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        variables[Var] = newValue;

        return Task.CompletedTask;
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
