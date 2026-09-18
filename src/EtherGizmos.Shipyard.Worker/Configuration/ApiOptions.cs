using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Configuration;

public class ApiOptions
{
    [Required]
    public string BaseUrl { get; set; } = null!;

    [Required]
    public ApiOAuth2Options OAuth2 { get; set; } = new();

    public class ApiOAuth2Options
    {
        [Required]
        public string ClientId { get; set; } = null!;

        [Required]
        public string ClientSecret { get; set; } = null!;
    }
}
