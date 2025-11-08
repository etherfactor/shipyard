using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Api.Configuration;

public class LogIngestionOptions
{
    public Dictionary<string, LogIngestionSource> Sources { get; set; } = [];

    public class LogIngestionSource
    {
        [Required]
        public string ApiKey { get; set; } = null!;
    }
}
