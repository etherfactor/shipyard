using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class WebUIOptions
{
    [Required, Url]
    public string BaseUrl { get; set; } = null!;
}
