using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Models.Enums;

public enum OAuth2ApplicationType
{
    Unknown = 0,
    Native = 10,
    Web = 20,
}

public static class OAuth2ApplicationTypeConverter
{
    private static Dictionary<OAuth2ApplicationType, string?> Mappings { get; } = new()
    {
        [OAuth2ApplicationType.Native] = ApplicationTypes.Native,
        [OAuth2ApplicationType.Web] = ApplicationTypes.Web,
    };

    public static OAuth2ApplicationType FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2ApplicationType.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2ApplicationType? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2ApplicationType value)
    {
        if (value == OAuth2ApplicationType.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2ApplicationType? value)
    {
        if (value is null || value == OAuth2ApplicationType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
