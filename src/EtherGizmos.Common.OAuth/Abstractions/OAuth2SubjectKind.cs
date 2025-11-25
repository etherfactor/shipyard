namespace EtherGizmos.Common.Abstractions;

public enum OAuth2SubjectKind
{
    Unknown = 0,
    User = 1,
    Client = 2,
}

public static class OAuth2SubjectKindConverter
{
    private static Dictionary<OAuth2SubjectKind, string?> Mappings { get; } = new()
    {
        [OAuth2SubjectKind.User] = "user",
        [OAuth2SubjectKind.Client] = "client",
    };

    public static OAuth2SubjectKind FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2SubjectKind.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2SubjectKind? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2SubjectKind value)
    {
        if (value == OAuth2SubjectKind.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2SubjectKind? value)
    {
        if (value is null || value == OAuth2SubjectKind.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
