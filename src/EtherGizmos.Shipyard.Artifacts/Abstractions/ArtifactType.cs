namespace EtherGizmos.Shipyard.Abstractions;

public enum ArtifactType
{
    Text = 1,
    Webp = 10,
}

public static class ArtifactTypeExtensions
{
    public static string ToExtension(
        this ArtifactType @this)
    {
        return @this switch
        {
            ArtifactType.Text => "txt",
            ArtifactType.Webp => "webp",
            _ => throw new NotSupportedException()
        };
    }
}
