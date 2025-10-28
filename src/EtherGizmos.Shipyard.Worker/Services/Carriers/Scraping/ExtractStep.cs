using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ExtractStep : ScrapingStep, ISettableStep
{
    public override string StepName => "[DEPRECATED] Extract";

    [Required]
    public string Selector { get; set; } = null!;

    [Required]
    public string Var { get; set; } = null!;

    public bool Trim { get; set; } = false;

    public override async Task Apply(IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        var html = await client.GetHtmlAsync(cancellationToken);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        await Apply(document.DocumentNode, client, variables, results, cancellationToken);
    }

    protected internal override Task Apply(HtmlNode subNode, IBrowserClient client, IDictionary<string, object> variables, IDictionary<string, object> results, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Finding text in element {CssSelector}", Selector);

        var text = subNode.QuerySelector(Selector)?.InnerText ?? "";

        Logger.LogInformation("Found text {Content}", text);

        if (Trim)
        {
            text = text.Trim();
        }

        Logger.LogInformation("Setting variable {Variable} to value {Value}", Var, text);

        variables[Var] = text;

        return Task.CompletedTask;
    }
}
