using EtherGizmos.Shipyard.Worker.Services.WebDrivers;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers.Scraping;

internal class ReturnStep : ScrapingStep, IResultStep, IVariableStep
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Var { get; set; } = null!;

    public Dictionary<string, object> Results { get; set; } = null!;

    public Dictionary<string, object> Variables { get; set; } = null!;

    public override Task Apply(IBrowserClient client, CancellationToken cancellationToken = default)
    {
        var useValue = Variables.TryGetValue(Var, out var value) ? value ?? "" : "";

        Results[Name] = useValue;

        return Task.CompletedTask;
    }
}
