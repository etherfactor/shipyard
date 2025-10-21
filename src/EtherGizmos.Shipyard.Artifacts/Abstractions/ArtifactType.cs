namespace EtherGizmos.Shipyard.Abstractions;

public enum ArtifactType
{
    Text = 1,
    WebP = 10,
}

public static class ArtifactTypeExtensions
{
    public static string ToExtension(
        this ArtifactType @this)
    {
        return @this switch
        {
            ArtifactType.Text => "txt",
            ArtifactType.WebP => "webp",
            _ => throw new NotSupportedException()
        };
    }

    public static string ToContentType(
        this ArtifactType @this)
    {
        return @this switch
        {
            ArtifactType.Text => "text/plain; charset=utf-8",
            ArtifactType.WebP => "image/webp",
            _ => throw new NotSupportedException()
        };
    }
}
