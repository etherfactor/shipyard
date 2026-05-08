using Microsoft.AspNetCore.Http;

namespace EtherGizmos.Shipyard.Api;

public class WriteArtifactDTO
{
    public IFormFile File { get; set; } = default!;
}
