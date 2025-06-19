using Asp.Versioning;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EtherGizmos.Shipyard.OData.Configuration;

public class ODataOptions
{
    [Required]
    public ApiVersion DefaultApiVersion { get; set; } = null!;

    [Required]
    public List<string> VersionedRoutePrefixes { get; set; } = [];

    [Required]
    public Assembly ExecutingAssembly { get; set; } = null!;

    [Required]
    public List<Assembly> ModelAssemblies { get; set; } = [];
}
