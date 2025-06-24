using HtmlAgilityPack;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public interface IDocumentStep
{
    HtmlDocument Document { get; set; }
}
