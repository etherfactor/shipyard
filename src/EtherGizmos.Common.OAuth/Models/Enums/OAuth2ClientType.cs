using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EtherGizmos.Common.Models.Enums;

public enum OAuth2ClientType
{
    Unknown = 0,
    Public = 10,
    Confidential = 20,
}

public static class OAuth2ClientTypeConverter
{
    private static Dictionary<OAuth2ClientType, string?> Mappings { get; } = new()
    {
        [OAuth2ClientType.Public] = ClientTypes.Public,
        [OAuth2ClientType.Confidential] = ClientTypes.Confidential,
    };

    public static OAuth2ClientType FromString(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return OAuth2ClientType.Unknown;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static OAuth2ClientType? FromStringOrDefault(
        string? value)
    {
        if (!Mappings.ContainsValue(value))
            return null;

        return Mappings.Single(e => e.Value == value).Key;
    }

    public static string? ToString(
        OAuth2ClientType value)
    {
        if (value == OAuth2ClientType.Unknown)
            return "";

        if (!Mappings.ContainsKey(value))
            return "";

        return Mappings[value];
    }

    public static string? ToStringOrDefault(
        OAuth2ClientType? value)
    {
        if (value is null || value == OAuth2ClientType.Unknown)
            return null;

        if (!Mappings.ContainsKey(value.Value))
            return null;

        return Mappings[value.Value];
    }
}
