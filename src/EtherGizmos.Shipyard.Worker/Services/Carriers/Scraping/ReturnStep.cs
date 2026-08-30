using EtherGizmos.Shipyard.Services.WebDrivers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Services.Carriers.Scraping;

internal class ReturnStep : ScrapingStep
{
    public override string StepName => "[DEPRECATED] Return";

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Var { get; set; } = null!;

    public override Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var useValue = variables.TryGetValue(Var, out var value) ? value ?? "" : "";

        Logger.LogInformation("Setting return value {Return} to value {@Value}", Name, useValue);

        results[Name] = useValue;

        return Task.CompletedTask;
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        await Apply(client, variables, results, cancellationToken);
    }
}
