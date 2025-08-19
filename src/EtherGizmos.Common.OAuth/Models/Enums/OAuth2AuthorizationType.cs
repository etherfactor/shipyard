using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Models.Enums;

public enum OAuth2AuthorizationType
{
    Unknown = 0,
    Permanent = 10,
    AdHoc = 20,
}

public static class OAuth2AuthorizationTypeConverter
{
    private static Dictionary<OAuth2AuthorizationType, string?> Mappings { get; } = new()
    {
        [OAuth2AuthorizationType.Permanent] = AuthorizationTypes.Permanent,
        [OAuth2AuthorizationType.AdHoc] = AuthorizationTypes.AdHoc,
    };

    public static OAuth2AuthorizationType FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2AuthorizationType.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2AuthorizationType? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2AuthorizationType value)
    {
        if (value == OAuth2AuthorizationType.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2AuthorizationType? value)
    {
        if (value is null || value == OAuth2AuthorizationType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
