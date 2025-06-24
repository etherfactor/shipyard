namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public interface IResultStep
{
    Dictionary<string, object> Results { get; set; }
}
