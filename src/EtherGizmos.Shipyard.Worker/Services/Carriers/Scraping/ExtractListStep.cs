using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ExtractListStep : ScrapingStep
{
    public override string StepName => "[DEPRECATED] Extract List";

    [Required]
    public string Selector { get; set; } = null!;

    [Required]
    public string Var { get; set; } = null!;

    [Required]
    public List<ScrapingStep> Steps { get; set; } = null!;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var html = await client.GetHtmlAsync(cancellationToken);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        await Apply(document.DocumentNode, client, variables, results, cancellationToken);
    }

    protected internal override async Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Finding nodes matching {CssSelector}", Selector);

        var values = new List<object>();

        var nodes = subNode.QuerySelectorAll(Selector);

        Logger.LogInformation("Found {Count} nodes", nodes.Count);

        foreach (var node in nodes)
        {
            var subVariables = new Dictionary<string, object>();
            values.Add(subVariables);

            foreach (var step in Steps)
            {
                await step.Apply(node, client, subVariables, results, cancellationToken);
            }
        }

        variables[Var] = values;
    }
}
