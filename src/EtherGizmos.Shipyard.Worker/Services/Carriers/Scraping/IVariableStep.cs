namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

public interface IVariableStep
{
    Dictionary<string, object> Variables { get; set; }
}
