using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Models.Enums;

public enum OAuth2ConsentType
{
    Unknown = 0,
    Explicit = 20,
    External = 30,
    Implicit = 10,
    Systematic = 40,
}

public static class OAuth2ConsentTypeConverter
{
    private static Dictionary<OAuth2ConsentType, string?> Mappings { get; } = new()
    {
        [OAuth2ConsentType.Explicit] = ConsentTypes.Explicit,
        [OAuth2ConsentType.External] = ConsentTypes.External,
        [OAuth2ConsentType.Implicit] = ConsentTypes.Implicit,
        [OAuth2ConsentType.Systematic] = ConsentTypes.Systematic,
    };

    public static OAuth2ConsentType FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2ConsentType.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2ConsentType? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2ConsentType value)
    {
        if (value == OAuth2ConsentType.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2ConsentType? value)
    {
        if (value is null || value == OAuth2ConsentType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
