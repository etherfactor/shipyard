using Microsoft.AspNetCore.Http;

namespace EtherGizmos.Shipyard.Api;

public class ArtifactRequestDTO
{
    public IFormFile File { get; set; } = default!;
}
