namespace EtherGizmos.Shipyard.Abstractions;

public record ArtifactFormat(
    string ContentType,
    string Extension,
    bool ShouldGzip
)
{
    public static readonly ArtifactFormat Text = new("text/plain; charset=utf-8", "txt", true);
    public static readonly ArtifactFormat Json = new("application/json", "json", true);
    public static readonly ArtifactFormat NdJson = new("application/x-ndjson", "ndjson", true);
    public static readonly ArtifactFormat Html = new("text/html; charset=utf-8", "html", true);
    public static readonly ArtifactFormat WebP = new("image/webp", "webp", false);
    public static readonly ArtifactFormat Png = new("image/png", "png", false);

    public static readonly ArtifactFormat Binary = new("application/octet-stream", "bin", false);
}
