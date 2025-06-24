using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ExtractStep : ScrapingStep, ISettableStep
{
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
        var text = subNode.QuerySelector(Selector)?.InnerText ?? "";

        if (Trim)
        {
            text = text.Trim();
        }

        variables[Var] = text;

        return Task.CompletedTask;
    }
}
