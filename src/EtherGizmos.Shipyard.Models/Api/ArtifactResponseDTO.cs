namespace EtherGizmos.Shipyard.Api;

public class ArtifactResponseDTO
{
    public required string ArtifactUri { get; set; }

    public required string ContentType { get; set; }

    public required string FileName { get; set; }

    public required long Bytes { get; set; }
}
