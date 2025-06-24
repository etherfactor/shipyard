using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ExtractStep : ScrapingStep, IDocumentStep, ISettableStep, IVariableStep
{
    [Required]
    public string Selector { get; set; } = null!;

    [Required]
    public string Var { get; set; } = null!;

    public bool Trim { get; set; } = false;

    public HtmlDocument Document { get; set; } = null!;

    public Dictionary<string, object> Variables { get; set; } = null!;

    public override Task Apply(IBrowserClient client, CancellationToken cancellationToken = default)
    {
        var text = Document.DocumentNode.QuerySelector(Selector).InnerText;

        if (Trim)
        {
            text = text.Trim();
        }

        Variables[Var] = text;

        return Task.CompletedTask;
    }
}
