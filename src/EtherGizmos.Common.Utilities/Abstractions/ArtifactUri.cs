using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Abstractions;

public readonly record struct ArtifactUri
{
    private const string Prefix = "artifact://";

    public string Value { get; }

    public ArtifactUri(string value)
    {
        Value = Normalize(value);
    }

    public static explicit operator string(ArtifactUri uri)
        => uri.Value;

    public override string ToString()
        => Value;

    private static string Normalize(
        string value)
    {
        if (!value.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Artifact URI must start with prefix artifact://");

        value = string.Concat(value.AsSpan(0, Prefix.Length), new Regex("[^-0-9A-Za-z/]").Replace(value.Substring(Prefix.Length), ""));
        value = value.ToLower();

        return value;
    }

    public static bool TryParse(
        string input,
        out ArtifactUri result)
    {
        try
        {
            result = new ArtifactUri(input);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
