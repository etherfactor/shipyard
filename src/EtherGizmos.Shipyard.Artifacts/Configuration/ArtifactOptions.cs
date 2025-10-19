using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Configuration;

public class ArtifactOptions
{
    [Required]
    public string BasePath { get; set; } = null!;
}
