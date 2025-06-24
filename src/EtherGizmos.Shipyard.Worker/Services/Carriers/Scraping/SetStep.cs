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

    public HtmlDocument Document { get; set; } = null!;

    public Dictionary<string, object> Variables { get; set; } = null!;

    public override Task Apply(IBrowserClient client, CancellationToken cancellationToken = default)
    {
        var builder = new StringBuilder();

        var regex = new Regex(@"(?<!{){(?<key>[^{}]+)}(?!})");
        var newValue = regex.Replace(Value, match =>
        {
            var key = match.Groups["key"].Value;
            return Variables.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
        });

        Variables[Var] = newValue;

        return Task.CompletedTask;
    }
}
